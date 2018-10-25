using Data;
using NaughtyAttributes;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlanetGenerator : MonoBehaviour
{
    [Expandable]
    public PlanetSettings PlanetSettings;

    private Planet planet;

    #region settings foldout

    [HideInInspector]
    public bool PlanetSettingsFoldout = true;

    [HideInInspector]
    public bool ShapeSettingsFoldout = true;

    [HideInInspector]
    public bool NoiseSettingsFoldout = true;

    [HideInInspector]
    public bool ColourSettingsFoldout = true;

    #endregion

    private void OnEnable()
    {
        DestroyAllPlanets();
        GeneratePlanet();


        PlanetSettings.LODTarget = Camera.main.transform;
    }

    private void OnApplicationQuit()
    {
        DestroyAllPlanets();
    }

    [Button("Generate Planet")]
    public void GeneratePlanet()
    {
        var sw = new Stopwatch();
        sw.Start();

        if (planet == null)
        {
            planet = new GameObject("Procedural planet")
                .AddComponent<Planet>();
        }

        planet.Initialize(PlanetSettings)
              .GenerateMesh()
              .GenerateColors();

        sw.Stop();

        Debug.Log($"Planet generated in {sw.Elapsed:s\'.\'ff}s");
    }

    [Button]
    public static void DestroyAllPlanets()
    {
        var planets = FindObjectsOfType<Planet>().Select(p => p.gameObject).ToList();

        if (Application.isEditor)
        {
            planets.ForEach(DestroyImmediate);
        }
        else
        {
            planets.ForEach(Destroy);
        }
    }
}