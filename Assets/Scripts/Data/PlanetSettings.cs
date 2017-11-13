using UnityEngine;

namespace Data {
    [System.Serializable]
    public struct PlanetSettings
    {
        public int Size;
        public float Radius;
        public AnimationCurve HeightCurve;
        public float HeightModifier;
        public NoiseSettings NoiseSettings;
        public Material Material;
        
        public PlanetSettings(int size, float radius, AnimationCurve heightCurve, float heightModifier, NoiseSettings noiseSettings, Material material)
        {
            Size = size;
            Radius = radius;
            HeightCurve = heightCurve;
            HeightModifier = heightModifier;
            NoiseSettings = noiseSettings;
            Material = material;
        }
    }
}
