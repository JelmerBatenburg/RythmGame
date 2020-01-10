using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffect : MonoBehaviour
{
    public AudioLinesDisplay[] audioLines;
    public int samples;
    public float lineMultiplier;
    public float lineLerp;
    public GameObject audioVisualCube;
    public float cubeSize;

    public void Update()
    {
        DisplayAudioLines();
    }

    public void Start()
    {
        foreach (AudioLinesDisplay line in audioLines)
            for (int i = 0; i < samples / 2f; i++)
            {
                GameObject g = Instantiate(audioVisualCube, transform);
                line.cubes.Add(g.transform);
            }

    }

    public void DisplayAudioLines()
    {
        float[] spectrum = new float[samples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        foreach (AudioLinesDisplay line in audioLines)
            for (int i = 0; i < spectrum.Length / 2; i++)
            {
                float height = Mathf.Lerp(line.cubes[i].localScale.y, spectrum[i] * lineMultiplier, lineLerp);
                line.cubes[i].localScale = (Vector3.one * cubeSize) + (Vector3.up * height);
                line.cubes[i].position = Vector3.Lerp(line.beginPos, line.endPos, 1f / (spectrum.Length / 2f) * i) + (Vector3.up * (height / 2));
            }
    }

    [System.Serializable]
    public class AudioLinesDisplay
    {
        public Vector3 beginPos;
        public Vector3 endPos;
        public List<Transform> cubes = new List<Transform>();
    }
}
