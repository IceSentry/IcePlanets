using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int size;
    private float radius;

    public void Initialize(GameObject parent, int size, float radius, string name, Material material)
    {
        this.size = size;
        this.radius = radius;

        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;

        gameObject.name = name;
        
        GetComponent<MeshFilter>().mesh = mesh = new Mesh { name = "Procedural Face" };
        GetComponent<MeshRenderer>().materials = new []{ material };
        GetComponent<MeshCollider>().sharedMesh = mesh;

        Generate();
    }

    private void Generate()
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        var triangles = new int[size * size * 6];
        var uvs = new Vector2[vertices.Length];

        for (int i = 0, y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                for (int z = 0; z <= size; z++)
                {
                    var v3 = new Vector3(x, y, z);
                    v3 = v3 * new Vector3(1, 0, 1);
                    SetVertex(i, x, size, y);
                }

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
        mesh.normals = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
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
    }
}