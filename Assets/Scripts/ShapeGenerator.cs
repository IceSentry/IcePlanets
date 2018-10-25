using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private INoiseFilter[] noiseFilters;
    public float Min { get; private set; } = float.MaxValue;
    public float Max { get; private set; } = float.MinValue;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        Min = float.MaxValue;
        Max = float.MinValue;

        noiseFilters = new INoiseFilter[settings.NoiseLayers.Length];
        for (var i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new SimpleNoiseFilter(settings.NoiseLayers[i].Settings);
        }
    }

    public Vector3 CalculatePoint(Vector3 pointOnSphere)
    {
        float radius = settings.Radius;

        if (noiseFilters == null || noiseFilters.Length == 0)
        {
            return pointOnSphere;
        }

        float noiseValue = 0;
        float firstLayerValue = noiseFilters[0].Evaluate(pointOnSphere);

        if (settings.NoiseLayers[0].Enabled)
        {
            noiseValue = firstLayerValue;
        }

        for (int i = 1; i < settings.NoiseLayers.Length; i++)
        {
            if (!settings.NoiseLayers[i].Enabled)
            {
                continue;
            }

            float mask = settings.NoiseLayers[i].UseFirstLayerAsMask ? firstLayerValue : 1;
            noiseValue += noiseFilters[i].Evaluate(pointOnSphere) * mask;
        }

        var elevation = radius * (1 + noiseValue);
        CheckMinMax(elevation);

        return pointOnSphere * elevation;
    }

    private void CheckMinMax(float elevation)
    {
        if (elevation < Min)
        {
            Min = elevation;
        }

        if (elevation > Max)
        {
            Max = elevation;
        }
    }


}
