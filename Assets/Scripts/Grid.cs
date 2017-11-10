using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Grid : MonoBehaviour
{
    private int size;
    private float radius;

    private Mesh mesh;
    private Vector3[] vertices;
    private Color32[] colors;

    public void Initialize(GameObject parent, int size, float radius)
    {
        this.size = size;
        this.radius = radius;

        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;

        GetComponent<MeshFilter>().mesh = mesh = new Mesh { name = "Procedural Grid" };
        GetComponent<MeshRenderer>().materials[0] = new Material(Shader.Find("Custom/VertexColor"));
        GetComponent<MeshCollider>().sharedMesh = mesh;

        Generate();
    }

    private void Generate()
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        colors = new Color32[vertices.Length];
        var triangles = new int[size * size * 6];
        var uvs = new Vector2[vertices.Length];

        for (int i = 0, y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                SetVertex(i, x, y, 1);
                uvs[i] = new Vector2((float)x / size, (float)y / size);
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++)
        {
            for (int x = 0; x < size; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + size + 1;
                triangles[ti + 5] = vi + size + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.colors32 = colors;
        mesh.RecalculateNormals();
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

        vertices[i] = v * radius;

        //var noiseValue = noise.GetValueFractal(v.x, v.y, v.z);
        //vertices[i] = v.normalized * (radius + noiseValue * noisemod);
        //colors[i] = Color.Lerp(Color.blue, Color.green, noiseValue);
    }
}