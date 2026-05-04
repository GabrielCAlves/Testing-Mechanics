using UnityEngine;
using System.Collections.Generic;

public class Shadows : MonoBehaviour
{
    // The Object that'll use this script will only need:
    // - Sprite Renderer
    // - Shadow prefab (with the Solid script attached)
    // Note: The color of the shadow will be set by this script, so no need to set it in the prefab. Also make sure that this' sprite renderer's order in layer is bigger than the shadow's

    public static Shadows me;
    public GameObject shadow;
    public List<GameObject> pool = new List<GameObject>();
    private float cronometer;
    public float speed = 10f;
    public Color color = Color.white;

    private void Awake()
    {
        me = this;
    }

    public GameObject GetShadows()
    {
        for(int i = 0; i < pool.Count; ++i)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = transform.position;
                pool[i].transform.rotation = transform.rotation;
                pool[i].GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                pool[i].GetComponent<Solid>().myColor = color;
                return pool[i];
            }
        }

        GameObject newShadow = Instantiate(shadow, transform.position, transform.rotation) as GameObject;
        newShadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        newShadow.GetComponent<Solid>().myColor = color;
        pool.Add(newShadow);
        return newShadow;
    }

    public void ShadowsSkill()
    {
        cronometer += speed * Time.deltaTime;
        if(cronometer > 1)
        {
            GetShadows();
            cronometer = 0;
        }
    }
}
