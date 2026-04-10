using UnityEngine;

public class ShowHideList : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private string showAnimationName = "ANIM_Show";
    [SerializeField] private string hideAnimationName = "ANIM_Hide";
    [SerializeField] private GameObject showListArrow;
    private float directionArrowUp = 180f;
    private float directionArrowDown = 0f;

    private Animator animator;
    private bool isVisible = false;

    void Start()
    {
        if (targetObject != null)
        {
            animator = targetObject.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogWarning($"targetObject's Animator not found");
            }
        }
        else
        {
            Debug.LogWarning("targetObject not found!");
        }

        if (showListArrow == null)
        {
            Debug.LogWarning("showListArrow not found!");
        }
    }

    public void ShowHide()
    {
        if (animator == null)
        {
            return;
        }

        if (isVisible)
        {
            animator.Play(hideAnimationName);
            isVisible = false;

            FlipSprite(directionArrowDown);
        }
        else
        {
            animator.Play(showAnimationName);
            isVisible = true;

            FlipSprite(directionArrowUp);
        }
    }

    public void Show()
    {
        if (animator != null && !isVisible)
        {
            animator.Play(showAnimationName);
            isVisible = true;
        }
    }

    public void Hide()
    {
        if (animator != null && isVisible)
        {
            animator.Play(hideAnimationName);
            isVisible = false;
        }
    }

    void FlipSprite(float angle)
    {
        if (showListArrow == null) return;

        showListArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}