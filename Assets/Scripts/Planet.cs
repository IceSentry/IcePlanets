using System.Collections.Generic;
using System.Diagnostics;
using Data;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Directions = Face.Directions;

public class Planet : MonoBehaviour
{
    private PlanetSettings _planetSettings;

    private Face[] _faces;

    public Planet Initialize(PlanetSettings settings)
    {
        _planetSettings = settings;
        return this;
    }

    public void Generate()
    {
        if (_faces != null)
            DestroyAllFaces();

        _planetSettings.Noise = new Noise(_planetSettings.NoiseSettings);

        _faces = new Face[6];

        for (int i = 0; i < _faces.Length; i++)
        {
            _faces[i] = new GameObject().AddComponent<Face>();
            _faces[i]
                .Initialize(gameObject, _planetSettings, (Directions) i)
                .Split();
        }
    }

    private void DestroyAllFaces()
    {
        var faces = new List<GameObject>();
        for (var i = 0; i < _faces.Length; i++)
        {
            Face face = _faces[i];
            faces.Add(face.gameObject);
        }

        if (Application.isEditor)
            faces.ForEach(DestroyImmediate);
        else
            faces.ForEach(Destroy);
    }
}
