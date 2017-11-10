using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField]
    private int gridSize;

    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private float noisemod = 2;

    [SerializeField]
    private float frequency = 1;

    [SerializeField]
    private int seed = 1337;

    private FastNoise noise;

    private void Awake()
    {
        noise = new FastNoise(seed);
        noise.SetFrequency(frequency);

        Generate();
    }

    void Generate()
    {
        Debug.Log("Generate started");

        new GameObject().AddComponent<Grid>().Initialize(gameObject, gridSize, radius);

        Debug.Log("Generate finished");
    }
}