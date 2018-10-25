using Data;
using System;
using UnityEngine;
using Directions = Face.Directions;

public class Planet : MonoBehaviour
{
    public PlanetSettings PlanetSettings;
    private readonly ShapeGenerator shapeGenerator = new ShapeGenerator();
    private readonly ColorGenerator colorGenerator = new ColorGenerator();

    private Face[] faces;

    private bool hasFaces() => faces != null && faces.Length == 6;

    public float Error = 3.5f;

    public float MaxGeometricError = 16f;

    public Planet Initialize(PlanetSettings settings)
    {
        PlanetSettings = settings;
        shapeGenerator.UpdateSettings(PlanetSettings.ShapeSettings);
        colorGenerator.UpdateSettings(PlanetSettings.ColorSettings);

        if (!hasFaces())
        {
            faces = new Face[6];
            for (var i = 0; i < faces.Length; i++)
            {
                faces[i] = new GameObject()
                    .AddComponent<Face>();
            }
        }

        for (var i = 0; i < faces.Length; i++)
        {
            faces[i].Init(transform, PlanetSettings, (Directions) i, shapeGenerator)
                    .Generate()
                    .Split();
        }

        return this;
    }

    private void Update()
    {
        if (hasFaces())
        {
            foreach (var face in faces)
            {

                RenderLOD(face);
            }
        }
    }

    private float sigma(float x) => Math.Abs(x) < 0.00001f
                                       ? MaxGeometricError
                                       : sigma(x - 1) - sigma(x - 1) * x / 2f;

    private void RenderLOD(Face face)
    {
        Vector3 lodTarget = PlanetSettings.LODTarget.position;
        Vector3 center = face.Mesh.bounds.center;

        Debug.DrawLine(center, lodTarget, Color.magenta);

        float K = Screen.width / (2f * Mathf.Tan(Camera.main.fieldOfView / 2f));
        float D = Vector3.Distance(center, lodTarget);
        float s = sigma(face.LevelOfDetail);

        float p = (s / D) * K;

        face.Sigma = s;
        face.ErrorP = p;

        float tau = 3f;
        if (p <= tau)
        {
            //disable faces under this one and enable this one.
            face.Merge();
            face.Show();

            if (face.Direction == Directions.Front)
            {
                Debug.Log($"Level : {face.LevelOfDetail}, p: {p}");
            }
        }
        else
        {
            //disable this face, check the ones under it
            if (!hasFaces() || face.LevelOfDetail >= 16)
            {
                return;
            }

            face.Split();

            RenderLOD(face.SubFaces[0]);
            RenderLOD(face.SubFaces[1]);
            RenderLOD(face.SubFaces[2]);
            RenderLOD(face.SubFaces[3]);
        }
    }

    public Planet GenerateMesh()
    {
        foreach (var f in faces)
        {
            f.UpdateMesh();
        }

        colorGenerator.UpdateElevation(shapeGenerator.Min, shapeGenerator.Max);

        return this;
    }

    public Planet GenerateColors()
    {
        colorGenerator.UpdateColors();
        foreach (var f in faces)
        {
            f.UpdateColors();
        }

        return this;
    }
}
