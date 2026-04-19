using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private float timer = 3f;
    //[SerializeField] private bool enableTimer = false;
    [SerializeField] private float cloakTimer = 0f;

    [Header("Return Types")]
    [SerializeField] private bool returnNull = false;
    [SerializeField] private bool waitForSeconds = false;
    [SerializeField] private bool waitForSecondsRealTime = false;
    [SerializeField] private bool waitForEndOfFrame = false;
    [SerializeField] private bool waitUntil = false;
    [SerializeField] private bool waitWhile = false;
    [SerializeField] private bool activateAllCoroutines = false;

    [SerializeField] private GameObject objectFall;
    private Rigidbody rb;
    [SerializeField] private Vector3 objectInitialPosition;

    [Header("Wait Conditions")]
    [SerializeField] private int returnUntilCondition = 0; // Placeholder for WaitUntil condition
    [SerializeField] private int returnWhileCondition = 0; // Placeholder for WaitWhile condition


    private void Start()
    {
        UpdateTimeDisplay();

        if (objectFall != null) {
            rb = objectFall.GetComponent<Rigidbody>();
            rb.useGravity = false;
            objectInitialPosition = objectFall.transform.position;
        }
    }

    public void StartTimer()
    {
        //enableTimer = true;
        ListOfRunners.instance.CleanListOfRunners();

        SetInitialPosition();

        if (returnNull)
        {
            StartCoroutine(CountdownReturnNull());
        }

        if (waitForSeconds)
        {
            StartCoroutine(CountdownWaitForSeconds());
        }

        if (waitForSecondsRealTime)
        {
            StartCoroutine(CountdownWaitForSecondsRealTime());
        }

        if (waitForEndOfFrame)
        {
            StartCoroutine(CountdownWaitForEndOfFrame());
        }

        if (waitUntil)
        {
            StartCoroutine(CountdownWaitUntil());
        }

        if (waitWhile)
        {
            StartCoroutine(CountdownWaitWhile());
        }

        if(activateAllCoroutines)
        {
            StartCoroutine(CountdownReturnNull());
            StartCoroutine(CountdownWaitForSeconds());
            StartCoroutine(CountdownWaitForSecondsRealTime());
            StartCoroutine(CountdownWaitForEndOfFrame());
            StartCoroutine(CountdownWaitUntil());
            StartCoroutine(CountdownWaitWhile());
        }
    }

    public void PauseTimer()
    {
        //enableTimer = false;
        StopAllCoroutines();
    }

    private IEnumerator CountdownReturnNull()
    {
        while (cloakTimer < timer)
        {
            UpdateTimeDisplay();
            yield return null;
            cloakTimer += Time.deltaTime;
        }

        cloakTimer = 0f;
        UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private IEnumerator CountdownWaitForSeconds()
    {
        //while (cloakTimer < timer)
        //{
        //    UpdateTimeDisplay();
            yield return new WaitForSeconds(timer);
        //    cloakTimer += Time.deltaTime;
        //}

        //cloakTimer = 0f;
        //UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private IEnumerator CountdownWaitForSecondsRealTime()
    {
        //while (cloakTimer < timer)
        //{
        //    UpdateTimeDisplay();
            yield return new WaitForSecondsRealtime(timer);
        //    cloakTimer += Time.deltaTime;
        //}

        //cloakTimer = 0f;
        //UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private IEnumerator CountdownWaitForEndOfFrame()
    {
        while (cloakTimer < timer)
        {
            UpdateTimeDisplay();
            yield return new WaitForEndOfFrame();
            cloakTimer += Time.deltaTime;
        }

        cloakTimer = 0f;
        UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private IEnumerator CountdownWaitUntil()
    {
        while (cloakTimer < timer)
        {
            UpdateTimeDisplay();
            yield return new WaitUntil(() => returnUntilCondition > 0);
            cloakTimer += Time.deltaTime;
        }

        cloakTimer = 0f;
        UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private IEnumerator CountdownWaitWhile()
    {
        while (cloakTimer < timer)
        {
            UpdateTimeDisplay();
            yield return new WaitWhile(() => returnWhileCondition == 0);
            cloakTimer += Time.deltaTime;
        }

        cloakTimer = 0f;
        UpdateTimeDisplay();

        rb.useGravity = true;
    }

    private void UpdateTimeDisplay()
    {
        timeDisplay.text = string.Format("{0}:{1:00}", Mathf.FloorToInt((timer - cloakTimer) / 60), Mathf.FloorToInt((timer - cloakTimer) % 60));
    }

    private void SetInitialPosition()
    {
        if (objectFall != null && objectInitialPosition != null)
        {
            objectFall.transform.position = objectInitialPosition;
            rb.useGravity = false;
        }
    }
}
