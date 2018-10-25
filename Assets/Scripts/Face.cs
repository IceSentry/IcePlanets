using Data;
using NaughtyAttributes;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Face : MonoBehaviour
{
    public Mesh Mesh;

    private PlanetSettings planetSettings;

    private ShapeGenerator shapeGenerator;

    public Face[] SubFaces;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private FaceMeshGenerator meshGenerator;
    private FaceMeshData faceMeshData;

    public int LevelOfDetail;

    public Directions Direction;

    private Vector2 Offset;

    private const int numSubFaces = 4;

    public float Sigma;
    public float ErrorP;

    public enum Directions
    {
        Front,
        Back,
        Top,
        Bottom,
        Right,
        Left
    }

    private struct FaceData
    {
        public Transform parent;
        public PlanetSettings settings;
        public Face.Directions direction;
        public ShapeGenerator shapeGenerator;
    }

    private Face Init(FaceData face) => Init(face.parent, face.settings, face.direction, face.shapeGenerator);

    public Face Init(Transform parent, PlanetSettings settings, Directions direction, ShapeGenerator shapeGenerator)
    {
        planetSettings = settings;
        Direction = direction;
        this.shapeGenerator = shapeGenerator;

        if (parent != null)
        {
            transform.parent = parent;
        }

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = Mesh = new Mesh { name = "Procedural Face" };
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        meshGenerator = null;

        gameObject.name = direction.ToString();

        return this;
    }

    private Face WithOffsetAndLOD(float x, float y, int lodLevel)
    {
        Offset = new Vector2(x, y);
        LevelOfDetail = lodLevel;

        if (lodLevel < 10)
        {
            return this;
        }

        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = Mesh;

        return this;
    }

    [Button]
    public void Split()
    {
        Hide();

        if (SubFaces?.Length != numSubFaces)
        {
            InitSubFaces();
        }
        else
        {
            foreach (Face subFace in SubFaces)
            {
                subFace.Show();
            }
        }
    }

    [Button]
    public void Merge()
    {
        if (SubFaces == null || SubFaces.Length == 0)
        {
            return;
        }

        foreach (var face in SubFaces)
        {
            face.Merge();
            face.Hide();
        }

        Show();
    }

    [Button]
    public void Show()
    {
        SetVisibility(true);
    }

    [Button]
    public void Hide()
    {
        SetVisibility(false);
    }

    public void InitSubFaces()
    {
        SubFaces = new Face[numSubFaces];

        for (var i = 0; i < SubFaces.Length; i++)
        {
            SubFaces[i] = new GameObject().AddComponent<Face>();
        }


        int lodLevel = LevelOfDetail + 1;

        float tempXOffset = (Offset.x * 2f) + planetSettings.MeshResolution;
        float tempYOffset = (Offset.y * 2f) + planetSettings.MeshResolution;

        var currentFace = new FaceData
        {
            parent = transform,
            settings = planetSettings,
            direction = Direction,
            shapeGenerator = shapeGenerator
        };

        SubFaces[0]
            .Init(currentFace)
            .WithOffsetAndLOD(tempXOffset, tempYOffset, lodLevel)
            .Generate();
        SubFaces[1]
            .Init(currentFace)
            .WithOffsetAndLOD(tempXOffset, Offset.y * 2f, lodLevel)
            .Generate();
        SubFaces[2]
            .Init(currentFace)
            .WithOffsetAndLOD(Offset.x * 2f, Offset.y * 2f, lodLevel)
            .Generate();
        SubFaces[3]
            .Init(currentFace)
            .WithOffsetAndLOD(Offset.x * 2f, tempYOffset, lodLevel)
            .Generate();
    }

    public Face Generate()
    {
        UpdateMesh();
        UpdateColors();
        return this;
    }

    public void UpdateMesh()
    {
        if (meshGenerator == null)
        {
            faceMeshData = new FaceMeshData
            {
                Direction = Direction,
                Resolution = planetSettings.MeshResolution,
                Offset = Offset,
                LevelOfDetail = LevelOfDetail
            };

            meshGenerator = new FaceMeshGenerator(faceMeshData, shapeGenerator);
        }

        meshGenerator.UpdateMesh(Mesh);
    }

    public void UpdateColors() => meshRenderer.sharedMaterial = planetSettings.ColorSettings.Material;

    private void SetVisibility(bool visibility)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visibility;
        }

        if (meshCollider != null)
        {
            meshCollider.enabled = visibility;
        }

        gameObject.name = $"{Direction}{(visibility ? " - Visible" : "")}";
    }
}