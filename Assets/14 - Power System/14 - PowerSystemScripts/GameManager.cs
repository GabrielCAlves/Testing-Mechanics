using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject thermalCamera;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (thermalCamera == null)
        {
            var cameras = Camera.main.GetComponentsInChildren<Camera>();
            foreach (Camera camera in cameras)
            {
                if (camera.gameObject.name == "Thermal Camera")
                {
                    thermalCamera = camera.gameObject;
                    thermalCamera.SetActive(false); // Ensure the thermal camera is initially inactive
                }
            }
        }
    }
}
