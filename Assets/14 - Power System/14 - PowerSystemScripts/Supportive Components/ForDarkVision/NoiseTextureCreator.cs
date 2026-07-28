// Crie este script temporário para gerar a textura de ruído
using UnityEngine;

public class NoiseTextureCreator : MonoBehaviour
{
    [ContextMenu("Create Noise Texture")]
    void CreateNoiseTexture()
    {
        Texture2D texture = new Texture2D(256, 256);
        Color[] colors = new Color[256 * 256];

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                colors[y * 256 + x] = new Color(noise, noise, noise, 1);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        System.IO.File.WriteAllBytes("Assets/NoiseTexture.png", texture.EncodeToPNG());
        Debug.Log("Textura de ruído criada!");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}