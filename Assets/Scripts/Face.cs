using Data;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Color32[] _colors;

    private Directions _direction;
    private Noise _noise;

    [SerializeField]
    private PlanetSettings _planetSettings;

    private Face[] _subFaces;

    private Vector2 _localPos;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;

    public enum Directions
    {
        Front,
        Back,
        Top,
        Bottom,
        Right,
        Left
    }

    public Face Initialize(GameObject parent, PlanetSettings planetSettings, Directions direction)
    {
        _planetSettings = planetSettings;
        _direction = direction;
        _noise = planetSettings.Noise;
        
        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;

        gameObject.name = direction.ToString();

        return this;
    }

    public Face Initialize(GameObject parent, PlanetSettings planetSettings, Directions direction, Vector2 localPos)
    {
        Initialize(parent, planetSettings, direction);

        _localPos = localPos;

        _filter = gameObject.AddComponent<MeshFilter>();
        _renderer = gameObject.AddComponent<MeshRenderer>();
        _collider = gameObject.AddComponent<MeshCollider>();

        _filter.mesh = _mesh = new Mesh { name = "Procedural Face" };
        _renderer.materials = new[] { _planetSettings.Material };
        _collider.sharedMesh = _mesh;

        return this;
    }

    private void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }

    public void SubDivide()
    {
        if(_renderer != null)
            _renderer.enabled = false;
        if(_collider != null)
            _collider.enabled = false;

        var childPlanetSettings = _planetSettings;

        _subFaces = new Face[4];
        for (int i = 0; i < _subFaces.Length; i++)
        {
            _subFaces[i] = new GameObject().AddComponent<Face>();
        }

        _subFaces[0]
            .Initialize(gameObject, childPlanetSettings, _direction, new Vector2(0, 0))
            .Generate();
        _subFaces[1]
            .Initialize(gameObject, childPlanetSettings, _direction, new Vector2(1, 0))
            .Generate();
        _subFaces[2]
            .Initialize(gameObject, childPlanetSettings, _direction, new Vector2(0, 1))
            .Generate();
        _subFaces[3]
            .Initialize(gameObject, childPlanetSettings, _direction, new Vector2(1, 1))
            .Generate();
    }

    private void CreateVertices()
    {
        var halfSize = _planetSettings.MeshResolution / 2;

        _vertices = new Vector3[(halfSize + 1) * (halfSize + 1)];
        _normals = new Vector3[_vertices.Length];
        _colors = new Color32[_vertices.Length];
        var uvs = new Vector2[_vertices.Length];
      
        for (int i = 0, y = 0; y <= halfSize; y++)
        {
            for (int x = 0; x <= halfSize; x++, i++)
            {
                int tempX = x + (int)_localPos.x * halfSize;
                int tempY = y + (int)_localPos.y * halfSize;
                uvs[i] = new Vector2((float)x / _planetSettings.MeshResolution, (float)y / _planetSettings.MeshResolution);
                switch (_direction)
                {
                    case Directions.Front:
                        SetVertex(i, tempX, tempY, 0);
                        break;
                    case Directions.Back:
                        SetVertex(i, tempX, tempY, _planetSettings.MeshResolution);
                        break;
                    case Directions.Top:
                        SetVertex(i, tempX, _planetSettings.MeshResolution, tempY);
                        break;
                    case Directions.Bottom:
                        SetVertex(i, tempX, 0, tempY);
                        break;
                    case Directions.Right:
                        SetVertex(i, _planetSettings.MeshResolution, tempY, tempX);
                        break;
                    case Directions.Left:
                        SetVertex(i, 0, tempY, tempX);
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
        var halfSize = _planetSettings.MeshResolution / 2;

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
        Vector3 v = new Vector3(x, y, z) * 2f / _planetSettings.MeshResolution - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        v.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        v.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        v.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        _normals[i] = v;
        //_vertices[i] = s * _planetSettings.Radius;

        //TODO move this out of here
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