using Data;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Color32[] _colors;

    private Directions _direction;
    private CustomNoise _noise;

    private PlanetSettings _planetSettings;

    [SerializeField]
    private Gradient _planetGradient;

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

        _planetGradient = CreateColourGradient();

        Generate();
    }

    private void Generate()
    {
        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        _vertices = new Vector3[(_planetSettings.Size + 1) * (_planetSettings.Size + 1)];
        _colors = new Color32[_vertices.Length];

        var uvs = new Vector2[_vertices.Length];

        for (int i = 0, y = 0; y <= _planetSettings.Size; y++)
        {
            for (int x = 0; x <= _planetSettings.Size; x++, i++)
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
        var triangles = new int[_planetSettings.Size * _planetSettings.Size * 6];

        for (int ti = 0, vi = 0, y = 0; y < _planetSettings.Size; y++, vi++)
        {
            for (int x = 0; x < _planetSettings.Size; x++, vi++)
            {
                switch (_direction)
                {
                    case Directions.Front:
                    case Directions.Top:
                    case Directions.Right:
                        ti = SetQuad(triangles, ti, vi, vi + 1, vi + _planetSettings.Size + 1, vi + _planetSettings.Size + 2);
                        break;
                    case Directions.Back:
                    case Directions.Bottom:
                    case Directions.Left:
                        ti = SetQuad(triangles, ti, vi + _planetSettings.Size + 2, vi + 1, vi + _planetSettings.Size + 1, vi);
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
        Vector3 s;
        v.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        v.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        v.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        var noiseValue = _noise.GetValueAt(v.x, v.y, v.z);

        var waterlevel = 0.2f;
        //if (noiseValue > waterlevel)
        //    noiseValue = (noiseValue - waterlevel) * (1.0f / (1.0f - waterlevel));
        //else
        //    noiseValue = 0.0f;

        //_vertices[i] = v * (_planetSettings.Radius + (noiseValue * _planetSettings.HeightModifier));
        _colors[i] = _planetGradient.Evaluate(noiseValue);

        _vertices[i] = v * (_planetSettings.Radius + _planetSettings.HeightCurve.Evaluate(noiseValue) * _planetSettings.HeightModifier);

        if (noiseValue < 0.5f)
            _colors[i] = Color.blue;
        else if (noiseValue < 0.55f)
            _colors[i] = Color.yellow;
        else if (noiseValue < 0.70f)
            _colors[i] = Color.green;
        else if (noiseValue < 0.86f)
            _colors[i] = Color.gray;
        else
            _colors[i] = Color.white;
    }

    public Gradient CreateColourGradient()
    {
        var planetColors = new Gradient();

        GradientColorKey[] gck = new GradientColorKey[8];
        gck[0].color = CalculateGradientColour(0, 0, 128);
        gck[0].time = CalculateGradientTime(-1.0000);
        gck[1].color = CalculateGradientColour(0, 0, 255);
        gck[1].time = CalculateGradientTime(-0.2500);
        gck[2].color = CalculateGradientColour(0, 128, 255);
        gck[2].time = CalculateGradientTime(0.0000);
        gck[3].color = CalculateGradientColour(240, 240, 64);
        gck[3].time = CalculateGradientTime(0.0625);
        gck[4].color = CalculateGradientColour(32, 160, 0);
        gck[4].time = CalculateGradientTime(0.1250);
        gck[5].color = CalculateGradientColour(224, 224, 0);
        gck[5].time = CalculateGradientTime(0.3750);
        gck[6].color = CalculateGradientColour(128, 128, 128);
        gck[6].time = CalculateGradientTime(0.7500);
        gck[7].color = CalculateGradientColour(255, 255, 255);
        gck[7].time = CalculateGradientTime(1.0000);

        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gak[0].alpha = 1f;
        gak[0].time = 0f;
        gak[1].alpha = 1f;
        gak[1].time = 1f;

        planetColors.SetKeys(gck, gak);

        return planetColors;
    }


    Color CalculateGradientColour(int r, int g, int b)
    {
        return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
    }


    float CalculateGradientTime(double t)
    {
        return (float)((t + 1) * 0.5);
    }

}