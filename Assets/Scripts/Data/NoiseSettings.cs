namespace Data {
    [System.Serializable]
    public struct NoiseSettings
    {
        public float Frequency;
        public float Lacunarity;
        public int Octaves;
        public float Amplitude;
        public int Seed;

        public NoiseSettings(float frequency, float lacunarity, int octaves, float amplitude, int seed)
        {
            Frequency = frequency;
            Lacunarity = lacunarity;
            Octaves = octaves;
            Amplitude = amplitude;
            Seed = seed;
        }
    }
}
