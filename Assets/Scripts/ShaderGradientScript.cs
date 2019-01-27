using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderGradientScript : MonoBehaviour
{
    public Color startColor = Color.red;
    public Color endColor = Color.blue;

    public Color startColorDay = Color.red;
    public Color endColorDay = Color.blue;

    private float time = 0.0f;
    private Mesh mesh;
    private Color[] colors;
    public float cycleSeconds = 30.0f;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        colors = new Color[mesh.vertices.Length];
        colors[0] = startColorDay;
        colors[1] = endColorDay;
        colors[2] = startColorDay;
        colors[3] = endColorDay;
        mesh.colors = colors;
    }

    void Update()
    {
        time += Time.deltaTime;

        Color lerpColourStart = Color.Lerp(startColorDay, startColor, Mathf.PingPong(time / cycleSeconds, 1));
        Color lerpColourEnd = Color.Lerp(endColorDay, endColor, Mathf.PingPong(time / cycleSeconds, 1));

        colors[0] = lerpColourStart;
        colors[1] = lerpColourEnd;
        colors[2] = lerpColourStart;
        colors[3] = lerpColourEnd;
        mesh.colors = colors;
    }
}
