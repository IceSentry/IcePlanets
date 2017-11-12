using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private Color32[] colors;
    private int size;
    private float radius;
    private Directions direction;
    private FastNoise noise;


    public enum Directions
    {
        FRONT,
        BACK,
        TOP,
        BOTTOM,
        RIGHT,
        LEFT
    }

    public void Initialize(GameObject parent, int size, float radius, Material material, FastNoise noise, Directions direction)
    {
        this.size = size;
        this.radius = radius;
        this.direction = direction;
        this.noise = noise;

        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;

        gameObject.name = direction.ToString();

        GetComponent<MeshFilter>().mesh = mesh = new Mesh { name = "Procedural Face" };
        GetComponent<MeshRenderer>().materials = new[] { material };
        GetComponent<MeshCollider>().sharedMesh = mesh;

        Generate();
    }

    private void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        colors = new Color32[vertices.Length];

        var uvs = new Vector2[vertices.Length];

        for (int i = 0, y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                uvs[i] = new Vector2((float)x / size, (float)y / size);
                switch (direction)
                {
                    case Directions.FRONT:
                        SetVertex(i, x, y, 0);
                        break;
                    case Directions.BACK:
                        SetVertex(i, x, y, size);
                        break;
                    case Directions.TOP:
                        SetVertex(i, x, size, y);
                        break;
                    case Directions.BOTTOM:
                        SetVertex(i, x, 0, y);
                        break;
                    case Directions.RIGHT:
                        SetVertex(i, size, y, x);
                        break;
                    case Directions.LEFT:
                        SetVertex(i, 0, y, x);
                        break;
                    default:
                        continue;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.normals = vertices;
        mesh.colors32 = colors;
        mesh.uv = uvs;
    }

    private void CreateTriangles()
    {
        var triangles = new int[size * size * 6];

        for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++)
        {
            for (int x = 0; x < size; x++, vi++)
            {
                switch (direction)
                {
                    case Directions.FRONT:
                    case Directions.TOP:
                    case Directions.RIGHT:
                        ti = SetQuad(triangles, ti, vi, vi + 1, vi + size + 1, vi + size + 2);
                        break;
                    case Directions.BACK:
                    case Directions.BOTTOM:
                    case Directions.LEFT:
                        ti = SetQuad(triangles, ti, vi + size + 2, vi + 1, vi + size + 1, vi);
                        break;
                    default:
                        continue;
                }
            }
        }

        mesh.triangles = triangles;
    }

    private int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        //Spherify
        Vector3 v = new Vector3(x, y, z) * 2f / size - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        v.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        v.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        v.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        var noiseValue = noise.GetValueFractal(v.x, v.y, v.z);
        vertices[i] = v.normalized * (radius + noiseValue * 8f);
        colors[i] = Color.Lerp(Color.blue, Color.green, noiseValue);
    }
}