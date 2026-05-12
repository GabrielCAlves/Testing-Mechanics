using UnityEngine;
using System.Collections.Generic;

public class Shadows : MonoBehaviour
{
    // The Object that'll use this script will only need:
    // - Sprite Renderer
    // - Shadow prefab (with the Solid script attached)
    // Note: The color of the shadow will be set by this script, so no need to set it in the prefab. 
    // Also make sure that this' sprite renderer's order in layer is bigger than the shadow's

    public GameObject shadow;
    public float speed = 10f;
    public Color _color;

    private List<GameObject> pool = new List<GameObject>();
    private float cronometer;
    private SpriteRenderer myspriteRenderer;

    private void Start()
    {
        myspriteRenderer = GetComponent<SpriteRenderer>();

        // If the color is not set in the Inspector, use the color of the SpriteRenderer as default
        if (_color == default(Color))
        {
            _color = myspriteRenderer.color;
        }
    }

    private void Update()
    {
        ShadowsSkill();
    }

    private GameObject GetShadows()
    {
        for (int i = 0; i < pool.Count; ++i)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = transform.position;
                pool[i].transform.rotation = transform.rotation;
                pool[i].GetComponent<SpriteRenderer>().sprite = myspriteRenderer.sprite;

                Solid solidComponent = pool[i].GetComponent<Solid>();
                if (solidComponent != null)
                {
                    solidComponent.myColor = _color;
                }

                pool[i].transform.localScale = transform.localScale;
                return pool[i];
            }
        }

        GameObject newShadow = Instantiate(shadow, transform.position, transform.rotation);
        newShadow.GetComponent<SpriteRenderer>().sprite = myspriteRenderer.sprite;

        Solid newSolidComponent = newShadow.GetComponent<Solid>();
        if (newSolidComponent != null)
        {
            newSolidComponent.myColor = _color;
        }

        newShadow.transform.localScale = transform.localScale;
        pool.Add(newShadow);
        return newShadow;
    }

    public void ShadowsSkill()
    {
        cronometer += speed * Time.deltaTime;
        if (cronometer >= 1f)
        {
            GetShadows();
            cronometer = 0;
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject shadowObj in pool)
        {
            if (shadowObj != null)
            {
                Destroy(shadowObj);
            }
        }
        pool.Clear();
    }
}