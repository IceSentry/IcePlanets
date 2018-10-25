using Data;
using DefaultNamespace;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    private readonly FastNoise fastNoise;
    public readonly NoiseSettings NoiseSettings;

    public RigidNoiseFilter(NoiseSettings settings)
    {
        NoiseSettings = settings;

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
        int scale = NoiseSettings.Scale;
        float value = fastNoise.GetNoise(point.x * scale, point.y * scale, point.z * scale);

        value = 1 - Mathf.Abs(value);
        value *= value;

        return Mathf.Max(0, value - NoiseSettings.MinValue);
    }
}
