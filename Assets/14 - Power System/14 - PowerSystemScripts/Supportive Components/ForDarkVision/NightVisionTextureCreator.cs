using UnityEngine;

public class NightVisionTextureCreator : MonoBehaviour
{
    [ContextMenu("Create Night Vision Texture")]
    void CreateTexture()
    {
        Texture2D texture = new Texture2D(512, 512);
        Color[] colors = new Color[512 * 512];

        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                // Padrão de visão noturna (ruído + retícula)
                float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                float vignette = 1 - Mathf.Sqrt(Mathf.Pow(x / 256f - 1, 2) + Mathf.Pow(y / 256f - 1, 2));
                float scanline = Mathf.Sin(y * 0.2f) * 0.5f + 0.5f;

                // Cor verde com transparência
                float alpha = (noise * 0.3f + vignette * 0.2f + scanline * 0.1f) * 0.5f;
                colors[y * 512 + x] = new Color(0, 0.3f + noise * 0.2f, 0, alpha);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        // Salva como asset
        System.IO.File.WriteAllBytes("Assets/NightVisionTexture.png", texture.EncodeToPNG());
        Debug.Log("Textura criada em Assets/NightVisionTexture.png");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}