using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffect : MonoBehaviour
{
    public AudioLinesDisplay[] audioLines;
    public int samples;
    public float lineMultiplier;
    public float lineLerp;

    public void Update()
    {
        DisplayAudioLines();
    }

    public void DisplayAudioLines()
    {
        float[] spectrum = new float[samples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        foreach (AudioLinesDisplay line in audioLines)
        {
            line.renderer.positionCount = spectrum.Length / 2;
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < spectrum.Length / 2; i++)
            {
                Vector3 pos = Vector3.Lerp(line.beginPos, line.endPos, 1f / (spectrum.Length / 2f) * i);
                pos += Vector3.up * lineMultiplier * spectrum[i];
                pos = Vector3.Lerp(pos, line.renderer.GetPosition(i), lineLerp);
                positions.Add(pos);
            }
            line.renderer.SetPositions(positions.ToArray());
        }
    }

    [System.Serializable]
    public class AudioLinesDisplay
    {
        public Vector3 beginPos;
        public Vector3 endPos;
        public LineRenderer renderer;
    }
}
