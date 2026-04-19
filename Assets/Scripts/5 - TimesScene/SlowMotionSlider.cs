using UnityEngine;
using UnityEngine.UI;

public class SlowMotionSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        if(slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    // Update is called once per frame
    public void UpdateTimeScale()
    {
        if(slider != null)
        {
            Time.timeScale = slider.value;
        }
    }
}
