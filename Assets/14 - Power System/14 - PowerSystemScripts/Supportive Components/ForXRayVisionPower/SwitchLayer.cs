using UnityEngine;

public class SwitchLayer : MonoBehaviour
{
    public LayerMask defaultLayer;
    public LayerMask xRayLayer;

    private bool xRayActive;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (xRayActive)
            {
                xRayActive = !xRayActive;

                int layerNum = (int) Mathf.Log(defaultLayer.value, 2);

                gameObject.layer = layerNum;

                if(transform.childCount > 0)
                {
                    SetLayerAllChildren(transform, layerNum);
                }
            }
            else
            {
                xRayActive = !xRayActive;

                int layerNum = (int) Mathf.Log(xRayLayer.value, 2);

                gameObject.layer = layerNum;

                if(transform.childCount > 0)
                {
                    SetLayerAllChildren(transform, layerNum);
                }
            }
        }
    }

    private void SetLayerAllChildren(Transform parent, int layer)
    {
        var children = parent.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }
}