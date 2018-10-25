using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu]
    public class PlanetSettings : ScriptableObject
    {
        [Range(2, 255)]
        public int MeshResolution = 124;

        [Expandable]
        public ShapeSettings ShapeSettings;

        [Expandable]
        public ColorSettings ColorSettings;

        public Transform LODTarget;
    }
}
