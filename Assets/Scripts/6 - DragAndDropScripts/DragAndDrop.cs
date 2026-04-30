using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private Vector3 mousePosition;

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePosition();
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }

    //public bool hovered;
    //public Vector2 originalPosition;

    //private void Start()
    //{
    //    originalPosition = transform.position;
    //}

    //private void OnMouseEnter()
    //{
    //    hovered = true;
    //}

    //private void OnMouseExit()
    //{
    //    hovered = false;
    //}

    //private void Update()
    //{
    //    if (Input.GetMouseButtonUp(0) && hovered)
    //    {
    //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        transform.position = mousePosition;
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        if (handleReleasePosition(transform.position))
    //        {
    //            // Successfully dropped on a valid target
    //            // Destroy object/implement other logic
    //        }
    //        else
    //        {
    //            // Not dropped on a valid target, return to original position
    //            transform.position = originalPosition;
    //        }
    //    }
    //}

    //public bool handleReleasePosition(Vector2 position)
    //{
    //    // Implement logic to check if the position is valid for dropping
    //    // For example, you can check if it overlaps with a target area
    //    return false;
    //}
}
