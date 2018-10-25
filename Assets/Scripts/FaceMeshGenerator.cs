
using System.Collections.Generic;
using UnityEngine;

public class FaceMeshGenerator
{
    private readonly FaceMeshData data;
    private readonly ShapeGenerator shapeGenerator;

    public FaceMeshGenerator(FaceMeshData data, ShapeGenerator shapeGenerator)
    {
        this.data = data;
        this.shapeGenerator = shapeGenerator;
    }

    /// <summary>
    /// Generates vertices and triangles, spherifies them and add them to the mesh
    /// </summary>
    /// <param name="mesh"></param>
    public void UpdateMesh(Mesh mesh)
    {
        int resolution = data.Resolution;

        var vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        var uvs = new Vector2[vertices.Length];

        float uvFactor = 1.0f / resolution;

        var triangles = new int[resolution * resolution * 6];
        var triangleIndex = 0;

        for (int i = 0, y = 0; y <= resolution; y++)
        {
            for (var x = 0; x <= resolution; x++, i++)
            {
                uvs[i] = new Vector2(x * uvFactor, y * uvFactor);

                vertices[i] = GetVertex(x, y);

                SetTriangles(i, x, y, ref triangleIndex, triangles);
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
    }



    /// <summary>
    /// Get a vertex that has been spherified and moved according to the shapegenerator
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Vector3 GetVertex(float x, float y)
    {
        Vector3 v;
        v.x = x + data.Offset.x;
        v.y = y + data.Offset.y;
        v.z = data.Resolution * Mathf.Pow(2, data.LevelOfDetail);
        return shapeGenerator.CalculatePoint(GetSpherifiedVertexPosition(v));
    }

    /// <summary>
    /// Sets the proper order of arguments to spherify the vertex
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private Vector3 GetSpherifiedVertexPosition(Vector3 v)
    {
        switch (data.Direction)
        {
            case Face.Directions.Front:
                return SpherifyVertex(new Vector3(v.x, v.y, 0));
            case Face.Directions.Back:
                return SpherifyVertex(new Vector3(v.x, v.y, v.z));
            case Face.Directions.Top:
                return SpherifyVertex(new Vector3(v.x, v.z, v.y));
            case Face.Directions.Bottom:
                return SpherifyVertex(new Vector3(v.x, 0, v.y));
            case Face.Directions.Right:
                return SpherifyVertex(new Vector3(v.z, v.y, v.x));
            case Face.Directions.Left:
                return SpherifyVertex(new Vector3(0, v.y, v.x));
            default:
                return v;
        }
    }

    /// <summary>
    /// This is the formula that turns the cube into a sphere and gives the squares a more proportional shape
    /// Taken from https://catlikecoding.com/unity/tutorials/cube-sphere/
    /// </summary>
    private float SpherifyVertexMapping(float x, float y, float z) =>
        x * Mathf.Sqrt(1f - (y / 2f) - (z / 2f) + (y * z / 3f));

    /// <summary>
    /// Use the spherifyVertexMapping to map the given vertex to a sphere
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    private Vector3 SpherifyVertex(Vector3 vertex)
    {
        Vector3 v = (vertex * 2f / (data.Resolution * Mathf.Pow(2, data.LevelOfDetail))) - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;

        v.x = SpherifyVertexMapping(v.x, y2, z2);
        v.y = SpherifyVertexMapping(v.y, x2, z2);
        v.z = SpherifyVertexMapping(v.z, x2, y2);

        return v;
    }

    private void SetTriangles(int i, float x, float y, ref int triangleIndex, IList<int> triangles)
    {
        int resolution = data.Resolution;

        if (x >= resolution || y >= resolution)
        {
            return;
        }

        switch (data.Direction)
        {
            case Face.Directions.Front:
            case Face.Directions.Top:
            case Face.Directions.Right:
                SetQuad(triangles, ref triangleIndex, i, i + 1, i + resolution + 1, i + resolution + 2);
                break;
            case Face.Directions.Back:
            case Face.Directions.Bottom:
            case Face.Directions.Left:
                SetQuad(triangles, ref triangleIndex, i + resolution + 2, i + 1, i + resolution + 1, i);
                break;
            default:
                return;
        }
    }

    private static void SetQuad(IList<int> triangles, ref int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        i += 6;
    }

}
