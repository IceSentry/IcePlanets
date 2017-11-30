using System;
using UnityEngine;

namespace Data {
    [Serializable]
    public struct PlanetSettings
    {
        public int MeshResolution;
        public float Radius;
        public float HeightModifier;
        public NoiseSettings NoiseSettings;
        public Noise Noise;
        public Material Material;
        [Range(-1,1)]
        public float WaterLevel;
        public Gradient Gradient;
        public Transform LODTarget;
    }
}
