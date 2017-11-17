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

        _faces = new Face[6];

        for (int i = 0; i < _faces.Length; i++)
        {
            _faces[i] = new GameObject().AddComponent<Face>();
            _faces[i].Initialize(gameObject, _planetSettings, (Directions) i, new Vector2(0,0));
            _faces[i].SubDivide();
        }
    }

    private void DestroyAllFaces()
    {
        var faces = new List<GameObject>();
        foreach (var child in _faces) faces.Add(child.gameObject);
        if (Application.isEditor)
            faces.ForEach(DestroyImmediate);
        else
            faces.ForEach(Destroy);
    }
}
