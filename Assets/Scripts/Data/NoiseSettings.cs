using System;
using UnityEngine;

namespace Data {
    [Serializable]
    public class NoiseSettings
    {
        public int Seed;

        public FastNoise.FractalType FractalType;
        public FastNoise.NoiseType NoiseType;

        [Range(0, 1)]
        public float Frequency;

        [Range(0, 10)]
        public float Lacunarity;

        [Range(1,9)]
        public int Octaves;

        [Range(0, 2)]
        public float Gain;

        [Range(0, 100)]
        public int Scale;

        [Range(0, 1)]
        public float HeightModifier = 1;

        [Range(0, 1)]
        public float MinValue;
    }
}
