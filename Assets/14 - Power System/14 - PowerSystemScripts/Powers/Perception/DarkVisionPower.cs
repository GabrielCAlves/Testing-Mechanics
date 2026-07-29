using System;
using UnityEngine;
//using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewDarkVisionPower", menuName = "Powers/Perception/Dark Vision Power")]
public class DarkVisionPower : Power
{
    public event System.Action OnDarkVisionActivated;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ChangeDarkVision(user);

        Debug.Log($"Dark Vision Ativada");
    }

    void ChangeDarkVision(GameObject user)
    {
        OnDarkVisionActivated?.Invoke(); // NightVisionController will subscribe to this event and toggle the night vision effect when the power is activated or deactivated.
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        ChangeDarkVision(user);

        Debug.Log("Dark Vision Desativada");
    }
}