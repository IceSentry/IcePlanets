using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Face))]
    public class FaceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var face = (Face)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Split"))
            {
                face.Split();
            }

            if (GUILayout.Button("Merge"))
            {
                face.Merge();
            }
        }
    }
}
