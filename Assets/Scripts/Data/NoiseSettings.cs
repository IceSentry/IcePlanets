using UnityEngine;

namespace Data {
    [System.Serializable]
    public struct NoiseSettings
    {
        [Range(0, 1)]
        public float Frequency;
        [Range(0, 10)]
        public float Lacunarity;
        [Range(1,9)]
        public int Octaves;
        [Range(0, 2)]
        public float Gain;
        public int Seed;
        public int Scale;
        public FastNoise.FractalType FractalType;
        public FastNoise.NoiseType NoiseType;
    }
}
