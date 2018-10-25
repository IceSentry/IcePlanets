using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    private ColorSettings settings;
    private Texture2D texture;
    private const int textureResolution = 50;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(float min, float max)
    {
        settings.Material.SetVector("_MinMax_", new Vector4(min, max));
    }

    public void UpdateColors()
    {
        var colors = new Color[textureResolution];

        for (int i = 0; i < textureResolution; i++)
        {
            colors[i] = settings.Gradient.Evaluate(i / (textureResolution - 1f));
        }

        texture.SetPixels(colors);
        texture.Apply();
        settings.Material.SetTexture("_texture", texture);
    }


}
