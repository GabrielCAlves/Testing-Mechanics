using UnityEngine;

namespace AcousticsLite
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    public class AcousticSource : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AudioListener _listener;
        [SerializeField] private LayerMask _occlusionLayers;

        [Header("Occlusion Settings")]
        [SerializeField, Tooltip("How fast to change the volume and cutoff")][Range(0.0f, 20.0f)] private float _lerpSpeed = 8f;

        [Header("Volumetric Source (Direct Path)")]
        [Tooltip("Size of the sound emitter.")]
        [SerializeField] private float _sourceRadius = 1.0f;
        [Tooltip("Number of rays to check direct visibility.")]
        [Range(1, 20)][SerializeField] private int _directRayCount = 8;

        [Header("Indirect / Diffraction")]
        [Tooltip("Number of indirect rays to detect a hole.")]
        [SerializeField] private int _indirectRayCount = 16;
        [Tooltip("Radius of indirect ray scattering.")]
        [SerializeField] private float _scatterRadius = 6.0f;
        [Tooltip("Volume penalty for indirect path.")]
        [SerializeField] private float _bouncePenalty = 0.2f;

        [Tooltip("How easily the sound wraps around corners. Lower = needs many rays to open up.")]
        [SerializeField, Range(1, 10)] private int _raysNeededForFullIndirect = 4;

        [Header("Audio Curves")]
        [SerializeField, Range(0f, 1f)] private float _minVolumeRatio = 0.3f;

        [Tooltip("Lowest frequency cutoff when fully occluded. Increase this if sound feels too 'underwater'.")]
        [SerializeField] private float _minCutoff = 1500f;

        private AudioSource _audioSource;
        private AudioLowPassFilter _lowPassFilter;

        private float _targetVolume;
        private float _targetCutoff;
        private float _initialMaxVolume;
        private float _lastCalculatedVisibility = 0f;

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _lowPassFilter = GetComponent<AudioLowPassFilter>();
            _initialMaxVolume = _audioSource.volume;

            if (_listener == null)
            {
                if (Camera.main != null) _listener = Camera.main.GetComponent<AudioListener>();
                if (_listener == null)
                {
#if UNITY_2023_1_OR_NEWER
                    _listener = FindAnyObjectByType<AudioListener>();
#else
                _listener = FindObjectOfType<AudioListener>();
#endif
                }
            }
        }

        void Update()
        {
            if (_listener == null) return;
            ProcessAudio();
            ApplySmoothValues();
        }

        /// <summary>
        /// Processes audio based on the visibility of the source to the listener.
        /// </summary>
        void ProcessAudio()
        {
            // Calculate the distance from the source to the listener
            float dist = Vector3.Distance(transform.position, _listener.transform.position);

            // Calculate the direct visibility of the source to the listener
            float directVisibility = CalculateDirectVisibility(dist);
            _lastCalculatedVisibility = directVisibility;

            // Calculate volume and cutoff curves based on direct visibility
            float volumeCurve = Mathf.Pow(directVisibility, 0.5f);
            float cutoffCurve = Mathf.Pow(directVisibility, 0.3f);

            // Calculate the minimum volume that the source can have
            float occludedVolumeFloor = _initialMaxVolume * _minVolumeRatio;

            // Calculate the final volume and cutoff based on the direct visibility
            float directVolume = Mathf.Lerp(occludedVolumeFloor, _initialMaxVolume, volumeCurve);
            float directCutoff = Mathf.Lerp(_minCutoff, 22000f, cutoffCurve);

            // Initialize the final volume and cutoff to the direct values
            float finalVolume = directVolume;
            float finalCutoff = directCutoff;

            // If the direct visibility is less than 0.95, calculate the indirect path
            if (directVisibility < 0.95f)
            {
                // Calculate the indirect factor based on the distance
                float indirectFactor = CalculateIndirectPath(dist);

                // If the indirect factor is greater than 0, calculate the indirect volume and cutoff
                if (indirectFactor > 0f)
                {
                    // Calculate the maximum indirect volume
                    float maxIndirectVolume = _initialMaxVolume * (1.0f - _bouncePenalty);
                    // Calculate the indirect volume based on the indirect factor
                    float indirectVolume = Mathf.Lerp(occludedVolumeFloor, maxIndirectVolume, indirectFactor);

                    // Set the final volume to the maximum of the direct and indirect volumes
                    finalVolume = Mathf.Max(directVolume, indirectVolume);

                    // Calculate the indirect cutoff based on the indirect factor
                    float indirectCutoff = Mathf.Lerp(directCutoff, 18000f, indirectFactor);
                    // Set the final cutoff to the maximum of the direct and indirect cutoffs
                    finalCutoff = Mathf.Max(directCutoff, indirectCutoff);
                }
            }

            // Set the target volume and cutoff based on the final values
            SetTarget(finalVolume, finalCutoff);
        }

        /// <summary>
        /// Calculates the direct visibility by casting multiple rays from the source to the listener.
        /// </summary>
        /// <param name="dist">The distance from the source to the listener.</param>
        /// <returns>The direct visibility as a value between 0 and 1.</returns>
        float CalculateDirectVisibility(float dist)
        {
            int clearCount = 0;
            for (int i = 0; i < _directRayCount; i++)
            {
                // Calculate the origin point of the raycast, which is the source position plus a random offset
                // within the source radius
                Vector3 origin = transform.position + (Random.insideUnitSphere * _sourceRadius);
                // Calculate the direction of the raycast, which is the listener position minus the origin
                Vector3 dir = _listener.transform.position - origin;

                // Cast the ray and check if it hits any objects in the occlusion layer
                if (!Physics.Raycast(origin, dir, dir.magnitude, _occlusionLayers))
                {
                    clearCount++;
                    // If the raycast is clear, draw a green line from the origin to the listener position
                    if (i == 0) Debug.DrawLine(origin, _listener.transform.position, Color.green);
                }
                else
                {
                    // If the raycast hits an object, draw a red line from the origin to the listener position
                    if (i == 0) Debug.DrawLine(origin, _listener.transform.position, Color.red);
                }
            }
            // Calculate the direct visibility as the number of clear raycasts divided by the total number of raycasts
            return clearCount / (float)_directRayCount;
        }

        /// <summary>
        /// Calculates the indirect path factor based on the number of valid bounce paths.
        /// </summary>
        /// <param name="distToPlayer">The distance from the source to the player.</param>
        /// <returns>The indirect path factor as a value between 0 and 1.</returns>
        float CalculateIndirectPath(float distToPlayer)
        {
            // The number of valid bounce paths
            int validPaths = 0;

            // Loop through the number of indirect rays
            for (int i = 0; i < _indirectRayCount; i++)
            {
                // Calculate a random offset within the scatter radius
                Vector3 randomOffset = Random.insideUnitSphere * _scatterRadius;
                // Calculate the target point of the raycast, which is the player position plus the random offset
                Vector3 targetPoint = _listener.transform.position + randomOffset;
                // Calculate the direction of the raycast, which is the target point minus the source position
                Vector3 dir = targetPoint - transform.position;

                // Cast the ray and check if it hits any objects in the occlusion layer
                if (Physics.Raycast(transform.position, dir, out RaycastHit bounceHit, distToPlayer * 2.0f, _occlusionLayers))
                {
                    // Calculate the bounce point of the raycast, which is the hit point plus the hit normal times 0.2
                    Vector3 bouncePoint = bounceHit.point + (bounceHit.normal * 0.2f);
                    // Calculate the direction from the bounce point to the player position
                    Vector3 dirToPlayer = _listener.transform.position - bouncePoint;

                    // Check if the path from the bounce point to the player position is clear of objects
                    if (!Physics.Raycast(bouncePoint, dirToPlayer, dirToPlayer.magnitude, _occlusionLayers))
                    {
                        // Increment the number of valid bounce paths
                        validPaths++;

                        if (validPaths == 1)
                        {
                            Debug.DrawLine(transform.position, bouncePoint, Color.yellow);
                            Debug.DrawLine(bouncePoint, _listener.transform.position, Color.yellow);
                        }
                    }
                }
            }

            // If there are any valid bounce paths, calculate the indirect path factor
            if (validPaths > 0)
            {
                return Mathf.Clamp01(validPaths / (float)_raysNeededForFullIndirect);
            }
            // If there are no valid bounce paths, return 0
            return 0f;
        }

        /// <summary>
        /// Sets the target volume and low pass filter cutoff frequency for the lerping functionality.
        /// </summary>
        /// <param name="vol">The target volume.</param>
        /// <param name="cut">The target low pass filter cutoff frequency.</param>
        void SetTarget(float vol, float cut)
        {
            /// <summary>
            /// The target volume for the lerping functionality.
            /// </summary>
            _targetVolume = vol;

            /// <summary>
            /// The target low pass filter cutoff frequency for the lerping functionality.
            /// </summary>
            _targetCutoff = cut;
        }

        /// <summary>
        /// Applies the smoothed volume and low pass filter cutoff values to the AudioSource and LowPassFilter.
        /// </summary>
        void ApplySmoothValues()
        {
            // Lerp the volume towards the target volume
            _audioSource.volume = Mathf.Lerp(_audioSource.volume, _targetVolume, Time.deltaTime * _lerpSpeed);

            // Lerp the low pass filter cutoff towards the target cutoff
            _lowPassFilter.cutoffFrequency = Mathf.Lerp(_lowPassFilter.cutoffFrequency, _targetCutoff, Time.deltaTime * _lerpSpeed);
        }

        /// <summary>
        /// Draws gizmos for the selected object
        /// </summary>
        void OnDrawGizmosSelected()
        {
            // Set the gizmo color based on the calculated visibility
            if (Application.isPlaying) Gizmos.color = Color.Lerp(Color.red, Color.green, _lastCalculatedVisibility);
            else Gizmos.color = Color.magenta;

            // Draw a wire sphere gizmo around the source position
            Gizmos.DrawWireSphere(transform.position, _sourceRadius);

            // If the listener is not null, draw a wire sphere gizmo around the listener position
            if (_listener != null)
            {
                // Set the gizmo color to a light blue
                Gizmos.color = new Color(0, 1, 1, 0.2f);
                // Draw the wire sphere gizmo around the listener position
                Gizmos.DrawWireSphere(_listener.transform.position, _scatterRadius);
            }
        }
    }
}
