using Data;
using UnityEngine;

public class Noise
{
    private readonly FastNoise _fastNoise;
    private readonly NoiseSettings _noiseSettings;

    public Noise(NoiseSettings settings)
    {
        _noiseSettings = settings;

        _fastNoise = new FastNoise(settings.Seed);
        _fastNoise.SetFrequency(settings.Frequency);
        _fastNoise.SetFractalLacunarity(settings.Lacunarity);
        _fastNoise.SetFractalOctaves(settings.Octaves);
        _fastNoise.SetFractalGain(settings.Gain);
        _fastNoise.SetFractalType(settings.FractalType);
        _fastNoise.SetNoiseType(settings.NoiseType);
    }

    public float GetValueAt(float x, float y, float z)
    {
        var value = _fastNoise.GetNoise(x * _noiseSettings.Scale, y * _noiseSettings.Scale, z * _noiseSettings.Scale);
        //return value;
        return (value + 1) / 2; //normalize the value from -1,1 to 0,1
    }
}
