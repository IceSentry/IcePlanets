using Data;
using UnityEngine;

public class CustomNoise
{
    private readonly FastNoise _fastNoise;

    public CustomNoise(NoiseSettings settings)
    {
        _fastNoise = new FastNoise(settings.Seed);
        _fastNoise.SetFrequency(settings.Frequency);
        _fastNoise.SetFractalLacunarity(settings.Lacunarity);
        _fastNoise.SetFractalOctaves(settings.Octaves);
        _fastNoise.SetFractalGain(settings.Amplitude);
    }

    public float GetValueAt(float x, float y, float z)
    {
        var value = _fastNoise.GetSimplexFractal(x, y, z);
        return (value + 1) / 2; //normalize the value from -1,1 to 0,1
    }

    public float GetNoise(Vector3 vertex, float noiseOffset, float octaves, float noiseScale)
    {
        // add offset
        vertex.x += noiseOffset;
        vertex.y += noiseOffset;
        vertex.z += noiseOffset;

        // fractal noise
        float amp = 1f;
        float noise = 0f;
        float gain = 1f;
        float factor = 0f;

        for (int i = 0; i < octaves; i++)
        {
            factor += 1f / gain;
            var calcVert = vertex * noiseScale * gain;
            noise += _fastNoise.GetSimplex(calcVert.x, calcVert.y, calcVert.z) * (amp / gain);
            gain *= 2f;
        }

        // normalize noise by octave iteration factor
        noise /= factor;

        return noise;
    }
}
