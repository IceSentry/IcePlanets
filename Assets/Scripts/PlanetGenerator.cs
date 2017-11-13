using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Data;

public class PlanetGenerator : MonoBehaviour
{
    public PlanetSettings PlanetSettings;

    private List<Planet> _planets;

    public void GeneratePlanet()
    {
        DestroyAllPlanets();

        var planet = new GameObject("Procedural planet").AddComponent<Planet>();
        planet.Initialize(PlanetSettings).Generate();
    }

    public void DestroyAllPlanets()
    {
        _planets = new List<Planet>();
        _planets = FindObjectsOfType<Planet>().ToList();

        var planets = _planets.Select(planet => planet.gameObject).ToList();
        if (Application.isEditor)
            planets.ForEach(DestroyImmediate);
        else
            planets.ForEach(Destroy);
    }
}