using System.Collections.Generic;
using System.Diagnostics;
using Data;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

        GenerateFaces();
        //gameObject.transform.position = new Vector3(0,0,_planetSettings.Radius);
    }

    private void GenerateFaces()
    {
        var front = new GameObject().AddComponent<Face>();
        front.Initialize(gameObject, _planetSettings, Face.Directions.Front);
        _faces[0] = front;

        var back = new GameObject().AddComponent<Face>();
        back.Initialize(gameObject, _planetSettings, Face.Directions.Back);
        _faces[1] = back;

        var top = new GameObject().AddComponent<Face>();
        top.Initialize(gameObject, _planetSettings, Face.Directions.Top);
        _faces[2] = top;

        var bottom = new GameObject().AddComponent<Face>();
        bottom.Initialize(gameObject, _planetSettings, Face.Directions.Bottom);
        _faces[3] = bottom;

        var left = new GameObject().AddComponent<Face>();
        left.Initialize(gameObject, _planetSettings, Face.Directions.Left);
        _faces[4] = left;

        var right = new GameObject().AddComponent<Face>();
        right.Initialize(gameObject, _planetSettings, Face.Directions.Right);
        _faces[5] = right;
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
