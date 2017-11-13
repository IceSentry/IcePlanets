using UnityEngine;
using System.Collections.Generic;
using Data;

public class PlanetGenerator : MonoBehaviour
{
    public PlanetSettings PlanetSettings;

    public void GeneratePlanet()
    {
        DestroyAllChildren();
        var planet = new GameObject("Procedural planet").AddComponent<Planet>();
        planet.transform.parent = gameObject.transform;

        planet.Initialize(PlanetSettings).Generate();
    }

    public void DestroyAllChildren()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        if (Application.isEditor)
            children.ForEach(DestroyImmediate);
        else
            children.ForEach(Destroy);
    }
}