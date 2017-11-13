namespace Data {
    [System.Serializable]
    public struct NoiseSettings
    {
        public float Frequency;
        public float Lacunarity;
        public int Octaves;
        public float Gain;
        public int Seed;
        public float Scale;
        public FastNoise.FractalType FractalType;
        public FastNoise.NoiseType NoiseType;
    }
}
