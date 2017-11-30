using System.Collections.Generic;
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

    private PlanetSettings _planetSettings;

    private Face[] _subFaces;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private MeshCollider _collider;

    private int _lodLevel;

    private float _xOffset;
    private float _yOffset;

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

    private Face Initialize(GameObject parent, PlanetSettings planetSettings, Directions direction, float xOffset, float yOffset, int lodLevel)
    {
        Initialize(parent, planetSettings, direction);

        _xOffset = xOffset;
        _yOffset = yOffset;

        _filter = gameObject.AddComponent<MeshFilter>();
        _renderer = gameObject.AddComponent<MeshRenderer>();
        if (lodLevel >= 10)
            _collider = gameObject.AddComponent<MeshCollider>();

        _filter.mesh = _mesh = new Mesh { name = "Procedural Face" };
        _renderer.materials = new[] { _planetSettings.Material };

        if (lodLevel >= 10)
            _collider.sharedMesh = _mesh;

        _lodLevel = lodLevel;

        return this;
    }

    void Update()
    {
        var distance = Vector3.Distance(_planetSettings.LODTarget.position, gameObject.transform.position);

        if (distance < _planetSettings.Radius * 2f && _lodLevel < 2)
        {
            if (_renderer != null)
                _renderer.enabled = true;
            Split();
        }
        else
        {
            if (_renderer != null)
                _renderer.enabled = false;
        }
    }

    private void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }

    public void Split()
    {
        if (_subFaces != null)
            return;
        if (_renderer != null)
            _renderer.enabled = false;
        if (_collider != null)
            _collider.enabled = false;

        var childPlanetSettings = _planetSettings;

        _subFaces = new Face[4];
        for (int i = 0; i < _subFaces.Length; i++)
        {
            _subFaces[i] = new GameObject().AddComponent<Face>();
        }

        var lodLevel = _lodLevel + 1;

        var tempXOffset = _xOffset * 2f + _planetSettings.MeshResolution;
        var tempYOffset = _yOffset * 2f + _planetSettings.MeshResolution;

        _subFaces[0]
            .Initialize(gameObject, childPlanetSettings, _direction, tempXOffset, tempYOffset, lodLevel)
            .Generate();
        _subFaces[1]
            .Initialize(gameObject, childPlanetSettings, _direction, tempXOffset, _yOffset * 2f, lodLevel)
            .Generate();
        _subFaces[2]
            .Initialize(gameObject, childPlanetSettings, _direction, _xOffset * 2f, _yOffset * 2f, lodLevel)
            .Generate();
        _subFaces[3]
            .Initialize(gameObject, childPlanetSettings, _direction, _xOffset * 2f, tempYOffset, lodLevel)
            .Generate();
    }

    public void Merge()
    {
        DestroyAllSubFaces();

        if (_renderer != null)
            _renderer.enabled = true;
        if (_collider != null)
            _collider.enabled = true;
    }

    private void DestroyAllSubFaces()
    {
        if (_subFaces == null)
            return;

        var faces = new List<GameObject>();
        for (var i = 0; i < _subFaces.Length; i++)
        {
            Face face = _subFaces[i];
            faces.Add(face.gameObject);
        }

        if (Application.isEditor)
            faces.ForEach(DestroyImmediate);
        else
            faces.ForEach(Destroy);

        _subFaces = null;
    }

    private void CreateVertices()
    {
        _vertices = new Vector3[(_planetSettings.MeshResolution + 1) * (_planetSettings.MeshResolution + 1)];
        _normals = new Vector3[_vertices.Length];
        _colors = new Color32[_vertices.Length];
        var uvs = new Vector2[_vertices.Length];

        var uvFactor = 1.0f / _planetSettings.MeshResolution;

        for (int i = 0, y = 0; y <= _planetSettings.MeshResolution; y++)
        {
            for (int x = 0; x <= _planetSettings.MeshResolution; x++, i++)
            {
                uvs[i] = new Vector2(x * uvFactor, y * uvFactor);

                var tempX = x + _xOffset;
                var tempY = y + _yOffset;
                var tempZ = _planetSettings.MeshResolution * Mathf.Pow(2, _lodLevel);

                switch (_direction)
                {
                    case Face.Directions.Front:
                        SetVertex(i, tempX, tempY, 0);
                        break;
                    case Face.Directions.Back:
                        SetVertex(i, tempX, tempY, tempZ);
                        break;
                    case Face.Directions.Top:
                        SetVertex(i, tempX, tempZ, tempY);
                        break;
                    case Face.Directions.Bottom:
                        SetVertex(i, tempX, 0, tempY);
                        break;
                    case Face.Directions.Right:
                        SetVertex(i, tempZ, tempY, tempX);
                        break;
                    case Face.Directions.Left:
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
        var triangles = new int[_planetSettings.MeshResolution * _planetSettings.MeshResolution * 6];

        for (int ti = 0, vi = 0, y = 0; y < _planetSettings.MeshResolution; y++, vi++)
        {
            for (int x = 0; x < _planetSettings.MeshResolution; x++, vi++)
            {
                switch (_direction)
                {
                    case Face.Directions.Front:
                    case Face.Directions.Top:
                    case Face.Directions.Right:
                        ti = SetQuad(triangles, ti, vi, vi + 1, vi + _planetSettings.MeshResolution + 1, vi + _planetSettings.MeshResolution + 2);
                        break;
                    case Face.Directions.Back:
                    case Face.Directions.Bottom:
                    case Face.Directions.Left:
                        ti = SetQuad(triangles, ti, vi + _planetSettings.MeshResolution + 2, vi + 1, vi + _planetSettings.MeshResolution + 1, vi);
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

    private void SetVertex(int i, float x, float y, float z)
    {
        //_vertices[i] = new Vector3(x, y, z);
        //return;

        //Spherify
        Vector3 v = (new Vector3(x, y, z) * 2f) / (_planetSettings.MeshResolution * Mathf.Pow(2, _lodLevel)) - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        v.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        v.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        v.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        _normals[i] = v;
        //_vertices[i] = v * _planetSettings.Radius;
        //return;

        //TODO move this out of here
        float noiseValue = _noise.GetValueAt(v.x, v.y, v.z);

        _colors[i] = _planetSettings.Gradient.Evaluate(noiseValue);

        _vertices[i] = v * (_planetSettings.Radius +
            noiseValue * (_planetSettings.HeightModifier / _planetSettings.Radius) * _planetSettings.Radius);
    }
}