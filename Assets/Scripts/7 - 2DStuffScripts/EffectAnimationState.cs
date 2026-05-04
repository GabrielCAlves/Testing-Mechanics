using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EffectAnimationState : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float permanentDuration = 5f;
    [SerializeField] private string endTrigger = "EndEffect";

    private void Awake()
    {
        gameObject.SetActive(true);
    }

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        StartCoroutine(EndAnimation());
    }

    IEnumerator EndAnimation()
    {
        if (animator == null)
            yield return null;

        yield return new WaitForSeconds(permanentDuration);
        animator.SetTrigger(endTrigger);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
