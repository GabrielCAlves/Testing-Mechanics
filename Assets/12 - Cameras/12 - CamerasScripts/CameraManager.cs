using DG.Tweening;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] virtualCameras;

    [SerializeField] private CinemachineCamera currentCam;
    [SerializeField] private CinemachineCamera startCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
    [SerializeField] private CinemachineCamera shootCam;
    [SerializeField] private CinemachineCamera topDownCam;
    [SerializeField] private CinemachineCamera downTopCam;

    private void Start()
    {
        currentCam = startCam;

        for(int i = 0; i < virtualCameras.Length; ++i)
        {
            if(virtualCameras[i] == currentCam)
            {
                virtualCameras[i].Priority = 10;
            }else
            {
                virtualCameras[i].Priority = 0;
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCamera(startCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCamera(shootCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCamera(thirdPersonCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchCamera(topDownCam);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchCamera(downTopCam);
        }
    }

    private void SwitchCamera(CinemachineCamera newCam)
    {
        currentCam.GetComponent<Transform>().gameObject.SetActive(false);

        newCam.GetComponent<Transform>().gameObject.SetActive(true);

        currentCam = newCam;

        //---------------------------------------------

        //newCam.Priority = currentCam.Priority + 1;

        //currentCam = newCam;

        //---------------------------------------------

        //newCam.Priority = 10;

        //currentCam = newCam;

        //foreach (CinemachineCamera c in virtualCameras)
        //{
        //    if ((c != currentCam && c.Priority != 0) || c != currentCam)
        //    {
        //        c.Priority = 0;
        //    }
        //}
    }
}