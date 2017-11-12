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

    private Face[] faces = new Face[6];

    private void Awake()
    {
        noise = new FastNoise(seed);
        noise.SetFrequency(frequency);

        GenerateFaces();
    }

    void GenerateFaces()
    {
        var front = new GameObject().AddComponent<Face>();
        front.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.FRONT);
        faces[0] = front;

        var back = new GameObject().AddComponent<Face>();
        back.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.BACK);
        faces[1] = back;

        var top = new GameObject().AddComponent<Face>();
        top.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.TOP);
        faces[2] = top;

        var bottom = new GameObject().AddComponent<Face>();
        bottom.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.BOTTOM);
        faces[3] = bottom;

        var left = new GameObject().AddComponent<Face>();
        left.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.LEFT);
        faces[4] = left;

        var right = new GameObject().AddComponent<Face>();
        right.Initialize(gameObject, gridSize, radius, material, noise, Face.Directions.RIGHT);
        faces[5] = right;

        //foreach (var face in faces)
        //{
        //    var mesh = face.GetComponent<MeshFilter>().mesh;
        //    var vertices = mesh.vertices;
        //    var normals = new Vector3[vertices.Length];
        //    var colors = new Color32[vertices.Length];
            
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        var v = vertices[i];
        //        var noiseValue = noise.GetValueFractal(v.x, v.y, v.z);
        //        vertices[i] = v.normalized * (radius + noiseValue * noisemod);
        //        normals[i] = vertices[i];
        //        colors[i] = Color.Lerp(Color.blue, Color.green, noiseValue);
        //    }

        //    mesh.vertices = vertices;
        //    mesh.colors32 = colors;
        //}
    }

}