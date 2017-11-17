using Data;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Color32[] _colors;

    private Directions _direction;
    private CustomNoise _noise;

    [SerializeField]
    private PlanetSettings _planetSettings;

    private Face[] _subFaces;

    public enum Directions
    {
        Front,
        Back,
        Top,
        Bottom,
        Right,
        Left
    }

    public void Initialize(GameObject parent, PlanetSettings planetSettings, Directions direction)
    {
        _planetSettings = planetSettings;
        _direction = direction;
        _noise = new CustomNoise(planetSettings.NoiseSettings);


        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;

        gameObject.name = direction.ToString();

        GetComponent<MeshFilter>().mesh = _mesh = new Mesh { name = "Procedural Face" };
        GetComponent<MeshRenderer>().materials = new[] { _planetSettings.Material };
        GetComponent<MeshCollider>().sharedMesh = _mesh;

        Generate();
    }

    private void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }

    public void SubDivide()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;

        var childPlanetSettings = _planetSettings;
        childPlanetSettings.Size *= 2;

        _subFaces = new[]
        {
            new GameObject().AddComponent<Face>(),
            new GameObject().AddComponent<Face>(),
            new GameObject().AddComponent<Face>(),
            new GameObject().AddComponent<Face>()
        };

        _subFaces[0].Initialize(gameObject, childPlanetSettings, _direction);
        _subFaces[1].Initialize(gameObject, childPlanetSettings, _direction);
        _subFaces[2].Initialize(gameObject, childPlanetSettings, _direction);
        _subFaces[3].Initialize(gameObject, childPlanetSettings, _direction);
    }

    private void CreateVertices()
    {
        var halfSize = _planetSettings.Size / 2;

        _vertices = new Vector3[(halfSize + 1) * (halfSize + 1)];
        _normals = new Vector3[_vertices.Length];
        _colors = new Color32[_vertices.Length];
        var uvs = new Vector2[_vertices.Length];
      
        for (int i = 0, y = 0; y <= halfSize; y++)
        {
            for (int x = 0; x <= halfSize; x++, i++)
            {
                uvs[i] = new Vector2((float)x / _planetSettings.Size, (float)y / _planetSettings.Size);
                switch (_direction)
                {
                    case Directions.Front:
                        SetVertex(i, x, y, 0);
                        break;
                    case Directions.Back:
                        SetVertex(i, x, y, _planetSettings.Size);
                        break;
                    case Directions.Top:
                        SetVertex(i, x, _planetSettings.Size, y);
                        break;
                    case Directions.Bottom:
                        SetVertex(i, x, 0, y);
                        break;
                    case Directions.Right:
                        SetVertex(i, _planetSettings.Size, y, x);
                        break;
                    case Directions.Left:
                        SetVertex(i, 0, y, x);
                        break;
                    default:
                        continue;
                }
            }
        }

        _mesh.vertices = _vertices;
        _mesh.normals = _vertices;
        _mesh.colors32 = _colors;
        _mesh.uv = uvs;
    }

    private void CreateTriangles()
    {
        var halfSize = _planetSettings.Size / 2;

        var triangles = new int[halfSize * halfSize * 6];

        for (int ti = 0, vi = 0, y = 0; y < halfSize; y++, vi++)
        {
            for (int x = 0; x < halfSize; x++, vi++)
            {
                switch (_direction)
                {
                    case Directions.Front:
                    case Directions.Top:
                    case Directions.Right:
                        ti = SetQuad(triangles, ti, vi, vi + 1, vi + halfSize + 1, vi + halfSize + 2);
                        break;
                    case Directions.Back:
                    case Directions.Bottom:
                    case Directions.Left:
                        ti = SetQuad(triangles, ti, vi + halfSize + 2, vi + 1, vi + halfSize + 1, vi);
                        break;
                    default:
                        continue;
                }
            }
        }

        _mesh.triangles = triangles;
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
        Vector3 v = new Vector3(x, y, z) * 2f / _planetSettings.Size - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        v.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        v.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        v.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        _normals[i] = v;
        //_vertices[i] = s * _planetSettings.Radius;

        var noiseValue = _noise.GetValueAt(v.x, v.y, v.z);

        _colors[i] = _planetSettings.Gradient.Evaluate(noiseValue);

        //Makes the water flat
        //var waterlevel = _planetSettings.WaterLevel;
        //if (noiseValue > waterlevel)
        //    noiseValue = (noiseValue - waterlevel) * (1.0f / (1.0f - waterlevel));
        //else
        //    noiseValue = 0.0f;

        _vertices[i] = v * (_planetSettings.Radius +
            noiseValue * (_planetSettings.HeightModifier / _planetSettings.Radius) * _planetSettings.Radius);
    }
}