using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{

    public int GridSize;

    public float Radius = 1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] cubeUV;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere";
        CreateVertices();
        CreateTriangles();
        CreateColliders();
    }

    private void CreateVertices()
    {
        const int cornerVertices = 8;
        int edgeVertices = (GridSize + GridSize + GridSize - 3) * 4;
        int faceVertices = (
            ((GridSize - 1) * (GridSize - 1))
            + ((GridSize - 1) * (GridSize - 1))
            + ((GridSize - 1) * (GridSize - 1))) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];

        var v = 0;
        for (var y = 0; y <= GridSize; y++)
        {
            for (var x = 0; x <= GridSize; x++)
            {
                SetVertex(v++, x, y, 0);
            }
            for (var z = 1; z <= GridSize; z++)
            {
                SetVertex(v++, GridSize, y, z);
            }
            for (int x = GridSize - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, GridSize);
            }
            for (int z = GridSize - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);
            }
        }
        for (var z = 1; z < GridSize; z++)
        {
            for (var x = 1; x < GridSize; x++)
            {
                SetVertex(v++, x, GridSize, z);
            }
        }
        for (var z = 1; z < GridSize; z++)
        {
            for (var x = 1; x < GridSize; x++)
            {
                SetVertex(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private static float VertexMapping(float x, float y, float z) =>
        x * Mathf.Sqrt(1f - (y / 2f) - (z / 2f) + (y * z / 3f));

    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 v = (new Vector3(x, y, z) * 2f / GridSize) - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;

        Vector3 s;
        s.x = VertexMapping(v.x, y2, z2);
        s.y = VertexMapping(v.y, x2, z2);
        s.z = VertexMapping(v.x, x2, y2);

        normals[i] = s;
        vertices[i] = normals[i] * Radius;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangles()
    {
        var size = GridSize * GridSize * 12;

        int[] trianglesZ = new int[size], trianglesX = new int[size], trianglesY = new int[size];

        int ring = (GridSize + GridSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        for (var y = 0; y < GridSize; y++, v++)
        {
            for (var q = 0; q < GridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (var q = 0; q < GridSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (var q = 0; q < GridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (var q = 0; q < GridSize - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * GridSize;
        for (var x = 0; x < GridSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (GridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (var z = 1; z < GridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + GridSize - 1);
            for (var x = 1; x < GridSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + GridSize - 1, vMid + GridSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + GridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (var x = 1; x < GridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        var v = 1;
        int vMid = vertices.Length - (GridSize - 1) * (GridSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (var x = 1; x < GridSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= GridSize - 2;
        int vMax = v + 2;

        for (var z = 1; z < GridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + GridSize - 1, vMin + 1, vMid);
            for (var x = 1; x < GridSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid + GridSize - 1, vMid + GridSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + GridSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (var x = 1; x < GridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private static int
    SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void CreateColliders()
    {
        gameObject.AddComponent<SphereCollider>();
    }

    //	private void OnDrawGizmos () {
    //		if (vertices == null) {
    //			return;
    //		}
    //		for (int i = 0; i < vertices.Length; i++) {
    //			Gizmos.color = Color.black;
    //			Gizmos.DrawSphere(vertices[i], 0.1f);
    //			Gizmos.color = Color.yellow;
    //			Gizmos.DrawRay(vertices[i], normals[i]);
    //		}
    //	}
}