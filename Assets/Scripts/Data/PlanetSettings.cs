using System;
using UnityEngine;

namespace Data {
    [Serializable]
    public struct PlanetSettings
    {
        public int Size;
        public float Radius;
        public float HeightModifier;
        public NoiseSettings NoiseSettings;
        public Material Material;
        [Range(-1,1)]
        public float WaterLevel;
        public Gradient Gradient;
    }
}
