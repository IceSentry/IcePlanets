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
        [Range(-1,1)]
        public float WaterLevel;
    }
}
