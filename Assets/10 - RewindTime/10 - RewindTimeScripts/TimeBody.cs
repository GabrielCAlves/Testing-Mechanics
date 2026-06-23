using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour
{
    [SerializeField] private float recordTime = 5f;
    [SerializeField] private KeyCode rewindKey = KeyCode.R;

    private bool isRewinding = false;
    private List<PointInTime> pointsInTime;
    private Rigidbody rb;

    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(rewindKey))
            StartRewind();
        if (Input.GetKeyUp(rewindKey))
            StopRewind();
    }

    private void FixedUpdate()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
    }

    private void Rewind()
    {
        if(pointsInTime.Count > 0)
        {
            // Get the first point in time from the list and set the position and rotation of the object to that point in time
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;

            // Removes the point in time that was just used
            pointsInTime.RemoveAt(0);
        }else
        {
            StopRewind();
        }
    }

    private void Record()
    {
        // If the list of points in time is greater than the amount of points we need to record, remove the oldest one.
        // recordTime / Time.fixedDeltaTime (time between FixedUpdate frames) equals the amount of points we need to record
        if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            // Remove the last point in time
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        // Insert the current position and rotation of the object at the beginning of the list (Stacking the list))
        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    public void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
    }
}
