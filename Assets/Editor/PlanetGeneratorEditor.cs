using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(PlanetGenerator))]
    public class PlanetGeneratorEditor : UnityEditor.Editor {

        public override void OnInspectorGUI()
        {
            var planetGenerator = (PlanetGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("GeneratePlanet"))
            {
                planetGenerator.GeneratePlanet();
            }

            if (GUILayout.Button("Destroy all planets"))
            {
                planetGenerator.DestroyAllPlanets();
            }
        }
    }
}
