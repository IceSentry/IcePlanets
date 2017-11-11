using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField]
    private int gridSize;

    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private float noisemod = 2;

    [SerializeField]
    private float frequency = 1;

    [SerializeField]
    private int seed = 1337;

    [SerializeField]
    private Material material;

    private FastNoise noise;

    private void Awake()
    {
        noise = new FastNoise(seed);
        noise.SetFrequency(frequency);

        Generate();
    }

    void Generate()
    {
        Debug.Log("Generate started");

        var faces = new List<Face>();

        var faceForward = new GameObject().AddComponent<Face>();
        faceForward.Initialize(gameObject, gridSize, radius, "forward", material);
        faces.Add(faceForward);

        //var faceBack = new GameObject().AddComponent<Face>();
        //faceBack.Initialize(gameObject, gridSize, radius, "back");
        //faces.Add(faceBack);

        var faceUp = new GameObject().AddComponent<Face>();
        faceUp.Initialize(gameObject, gridSize, radius, "up", material);
        faces.Add(faceUp);

        //new GameObject().AddComponent<Face>().Initialize(gameObject, gridSize, radius, "down");
        //new GameObject().AddComponent<Face>().Initialize(gameObject, gridSize, radius, "left");
        //new GameObject().AddComponent<Face>().Initialize(gameObject, gridSize, radius, "right");

        foreach (var face in faces)
        {
            var mesh = face.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;
            var normals = new Vector3[vertices.Length];
            var colors = new Color32[vertices.Length];
            
            for (int i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];
                var noiseValue = noise.GetValueFractal(v.x, v.y, v.z);
                vertices[i] = v.normalized * (radius + noiseValue * noisemod);
                normals[i] = vertices[i];
                colors[i] = Color.Lerp(Color.blue, Color.green, noiseValue);
            }

            mesh.vertices = vertices;
            mesh.colors32 = colors;
        }

        Debug.Log("Generate finished");
    }
}