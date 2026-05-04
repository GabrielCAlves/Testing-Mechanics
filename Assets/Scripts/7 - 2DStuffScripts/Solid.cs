using UnityEngine;

public class Solid : MonoBehaviour
{
    // Components for Shadow prefab:
    // - Sprite Renderer
    // - Animator (with its own Animator Controller, now called "Shadows", and ANIM_Shadow animation clip set, if necessary)
    // - Solid (this script)
    // Note: No need to set the color in here, it will be set by the Shadows script when the shadow is created

    [SerializeField] private SpriteRenderer myRenderer;
    [SerializeField] private Shader myShader;
    [SerializeField] public Color myColor = Color.white;

    private void Start()
    {
        if(myRenderer == null)
            myRenderer = GetComponent<SpriteRenderer>();
        
        if(myShader == null)
            myShader = Shader.Find("GUI/Text Shader");
    }

    private void ColorSprite()
    {
        myRenderer.material.shader = myShader;
        myRenderer.color = myColor;
    }

    public void Finish() 
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        ColorSprite();
    }
}
