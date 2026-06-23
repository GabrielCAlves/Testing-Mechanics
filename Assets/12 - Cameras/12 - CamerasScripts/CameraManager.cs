using DG.Tweening;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] virtualCameras;

    [SerializeField] private CinemachineCamera currentCam;
    [SerializeField] private CinemachineCamera startCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
    [SerializeField] private CinemachineCamera topDownCam;
    [SerializeField] private CinemachineCamera downTopCam;

    private void Start()
    {
        currentCam = startCam;

        for(int i = 0; i < virtualCameras.Length; ++i)
        {
            if(virtualCameras[i] == currentCam)
            {
                virtualCameras[i].Priority = 20;
            }else
            {
                virtualCameras[i].Priority = 10;
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            SwitchCamera(startCam);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SwitchCamera(topDownCam);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SwitchCamera(thirdPersonCam);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SwitchCamera(downTopCam);
        }
    }

    private void SwitchCamera(CinemachineCamera newCam)
    {
        currentCam.Priority = 10;

        currentCam = newCam;

        currentCam.Priority = 20;

        //for (int i = 0; i < virtualCameras.Length; ++i)
        //{
        //    if (virtualCameras[i] != currentCam)
        //    {
        //        virtualCameras[i].Priority = 10;
        //    }
        //}
    }
}