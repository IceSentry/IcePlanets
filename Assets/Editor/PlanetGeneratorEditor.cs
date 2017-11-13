using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        var mapGen = (PlanetGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("GeneratePlanet"))
        {
            mapGen.GeneratePlanet();
        }

        if (GUILayout.Button("Destroy all planets"))
        {
            mapGen.DestroyAllPlanets();
        }
    }
}
