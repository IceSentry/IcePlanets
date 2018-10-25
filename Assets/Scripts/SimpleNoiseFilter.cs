using Data;
using DefaultNamespace;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    private readonly FastNoise fastNoise;
    private readonly NoiseSettings settings;

    public SimpleNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;

        fastNoise = new FastNoise(settings.Seed);
        fastNoise.SetFrequency(settings.Frequency);
        fastNoise.SetFractalLacunarity(settings.Lacunarity);
        fastNoise.SetFractalOctaves(settings.Octaves);
        fastNoise.SetFractalGain(settings.Gain);
        fastNoise.SetFractalType(settings.FractalType);
        fastNoise.SetNoiseType(settings.NoiseType);
    }

    public float Evaluate(Vector3 point)
    {
        int scale = settings.Scale;
        float value = fastNoise.GetNoise(point.x * scale, point.y * scale, point.z * scale) * settings.HeightModifier;
        value = (value + 1) / 2;
        return Mathf.Max(0, value - settings.MinValue);
    }
}
