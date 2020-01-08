using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAudioData : MonoBehaviour
{
    public AudioSource source;
    public float visualSize;
    public float detectLevel;
    public int samples;
    public AudioTypeList audioTypes;
    private float hertzASample;
    public List<AudioRecordData> songData = new List<AudioRecordData>();
    public bool doneSampling;
    public float sampleDelay;
    public float beatStrenght;

    public void Start()
    {
        GetHertz();
        StartCoroutine(SamplingCheck());
    }

    public IEnumerator SamplingCheck()
    {
        while (true)
        {
            GetAudioTypes();
            if (!source.isPlaying)
                break;
            yield return new WaitForSeconds(sampleDelay);
        }
        doneSampling = true;
        Debug.Log("Done Sampling, Sampled a total of: " + songData.Count + " Samples");
    }

    public void CaluclateSamples()
    {

    }

    public void GetAudioTypes()
    {
        float[] spectrum = new float[samples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        AudioRecordData currentData = new AudioRecordData();

        for (int i = 0; i < spectrum.Length; i++)
        {
            float currentHertz = i * hertzASample;
            if (spectrum[i] < detectLevel)
                continue;

            if (currentHertz > audioTypes.brilliance)
            {
                if (currentData.brilliance == null || !currentData.brilliance.active || currentData.brilliance.strenght <= spectrum[i])
                    currentData.brilliance = new AudioRecordDataPart(true, spectrum[i]);
                continue;
            }
            else if (currentHertz > audioTypes.presence)
            {
                if (currentData.presence == null || !currentData.presence.active || currentData.presence.strenght <= spectrum[i])
                    currentData.presence = new AudioRecordDataPart(true, spectrum[i]);
                continue;
            }
            else if (currentHertz > audioTypes.upperMidrange)
            {
                if (currentData.upperMidrange == null || !currentData.upperMidrange.active || currentData.upperMidrange.strenght <= spectrum[i])
                    currentData.upperMidrange = new AudioRecordDataPart(true, spectrum[i]);
                continue;
            }
            else if (currentHertz > audioTypes.midrange)
            {
                if (currentData.midrange == null || !currentData.midrange.active || currentData.midrange.strenght <= spectrum[i])
                    currentData.midrange = new AudioRecordDataPart(true, spectrum[i]);
                continue;
            }
            else if (currentHertz > audioTypes.lowMidrange)
            {
                if (currentData.lowMidrange == null || !currentData.lowMidrange.active || currentData.lowMidrange.strenght <= spectrum[i])
                    currentData.lowMidrange = new AudioRecordDataPart(true, spectrum[i]);
                continue;
            }
            else if (currentHertz > audioTypes.subBass)
                GetComponent<MapEffect>().bassValue += beatStrenght * spectrum[i];
        }
        songData.Add(currentData);
    }

    public void GetHertz()
    {
        hertzASample = (AudioSettings.outputSampleRate / 2) / samples;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float[] spectrum = new float[samples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 0; i < (spectrum.Length - 1f) / 2f; i++)
            Gizmos.DrawCube(new Vector3(i * 1.4f, spectrum[i] * visualSize / 2, 0), new Vector3(1, spectrum[i] * visualSize, 1));
    }

    [System.Serializable]
    public class AudioTypeList
    {
        public float subBass;
        public float bass;
        public float lowMidrange;
        public float midrange;
        public float upperMidrange;
        public float presence;
        public float brilliance;
    }

    public class AudioRecordData
    {
        public AudioRecordDataPart lowMidrange;
        public AudioRecordDataPart midrange;
        public AudioRecordDataPart upperMidrange;
        public AudioRecordDataPart presence;
        public AudioRecordDataPart brilliance;
    }

    public class AudioRecordDataPart
    {
        public bool active = false;
        public float strenght = 0;

        public AudioRecordDataPart(bool _active, float _strenght)
        {
            active = _active;
            strenght = _strenght;
        }
    }
}
