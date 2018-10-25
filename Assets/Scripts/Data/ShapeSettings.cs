using System;
using Data;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    public NoiseLayer[] NoiseLayers;

    public float Radius = 250;

    [Serializable]
    public class NoiseLayer
    {
        public bool Enabled;
        public bool UseFirstLayerAsMask;
        public NoiseSettings Settings;
    }
}
