using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAudioData : MonoBehaviour
{
    [Header("Inputs")]
    public AudioSource source;
    public AudioClip clip;
    public AudioGameManager manager;

    [Header("Sampling")]
    public AudioTypeList audioTypes;
    public float detectLevel;
    public List<AudioRecordData> songData = new List<AudioRecordData>();
    public int sampleDelay;
    public float noteSpawnLevel;

    [Header("NoteSettings")]
    public float noteOffsetRange;
    public float noteSpawnWidth;
    public float noteSpawnHeight;
    public float heightStrenghtIntensity;
    public Transform noteSpawnPosition;

    public float noteLevelGain;
    public float noteLevelDrop;
    private float currentNoteLevel;
    public float bassDelay;

    [Header("Other")]
    public float visualSize;
    public GameData gameData;
    public GameObject samplingDisplay;

    public void Start()
    {
        StartCoroutine(NewCalculateData());
        samplingDisplay.SetActive(true);
        source.clip = clip;
    }

    public IEnumerator NewCalculateData()
    {
        clip.LoadAudioData();
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        int samplesLenght = data.Length / (AudioSettings.outputSampleRate / 2);

        float hertzASample = (AudioSettings.outputSampleRate / 2f) / samplesLenght;
        int rounds = data.Length / samplesLenght;
        float sampleLenght = clip.length / rounds;
        int i = 0;

        for (int currentRound = 0; currentRound < rounds; currentRound++)
        {
            AudioRecordData currentData = new AudioRecordData();
            currentData.time = currentRound * sampleLenght;
            for (int currentSample = 0; currentSample < samplesLenght; currentSample++)
            {
                int currentIndex = (currentRound * samplesLenght) + currentSample;
                float currentHertz = currentSample * hertzASample;

                if (data[currentIndex] < detectLevel)
                    continue;

                if (currentHertz > audioTypes.brilliance)
                {
                    if (currentData.brilliance == null || !currentData.brilliance.active || currentData.brilliance.strenght <= data[currentIndex])
                        currentData.brilliance = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentHertz > audioTypes.presence)
                {
                    if (currentData.presence == null || !currentData.presence.active || currentData.presence.strenght <= data[currentIndex])
                        currentData.presence = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentHertz > audioTypes.upperMidrange)
                {
                    if (currentData.upperMidrange == null || !currentData.upperMidrange.active || currentData.upperMidrange.strenght <= data[currentIndex])
                        currentData.upperMidrange = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentHertz > audioTypes.midrange)
                {
                    if (currentData.midrange == null || !currentData.midrange.active || currentData.midrange.strenght <= data[currentIndex])
                        currentData.midrange = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentHertz > audioTypes.lowMidrange)
                {
                    if (currentData.lowMidrange == null || !currentData.lowMidrange.active || currentData.lowMidrange.strenght <= data[currentIndex])
                        currentData.lowMidrange = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentHertz > audioTypes.bass)
                {
                    if (currentData.bass == null || !currentData.bass.active || currentData.bass.strenght <= data[currentIndex])
                        currentData.bass = new AudioRecordDataPart(true, data[currentIndex]);
                    continue;
                }
                else if (currentData.subBass == null || !currentData.subBass.active || currentData.subBass.strenght <= data[currentIndex])
                    currentData.subBass = new AudioRecordDataPart(true, data[currentIndex]);
            }

            songData.Add(currentData);

            if (i >= sampleDelay)
            {
                i = 0;
                yield return null;
                Debug.Log(currentRound + " / " + rounds);
            }
            i++;
        }
        Debug.Log("DoneSampling");
        CaluclateData();
    }

    public void CaluclateData()
    {
        AudioRecordData tempData = new AudioRecordData();
        gameData = new GameData();
        float currentValue = 0.5f;

        for (int i = 0; i < songData.Count; i++)
        {
            if (songData[i].subBass.active && !tempData.subBass.active)
            {
                if (gameData.bassAttack.Count == 0 || gameData.bassAttack[gameData.bassAttack.Count - 1] + bassDelay <= songData[i].time)
                    gameData.bassAttack.Add(songData[i].time);
                tempData.subBass.active = true;
            }
            tempData.subBass.active = songData[i].subBass.active;

            if (songData[i].bass.active && !tempData.bass.active)
            {
                if (gameData.bassAttack.Count == 0 || gameData.bassAttack[gameData.bassAttack.Count - 1] + bassDelay <= songData[i].time)
                    gameData.bassAttack.Add(songData[i].time);
                tempData.bass.active = true;
            }
            tempData.bass.active = songData[i].subBass.active;


            if (songData[i].midrange.active && !tempData.midrange.active)
            {
                if (songData[i].midrange.strenght >= currentNoteLevel)
                {
                    currentNoteLevel = songData[i].midrange.strenght + noteLevelGain;
                    currentValue += Random.Range(-noteOffsetRange, noteOffsetRange);
                    if (currentValue <= 0)
                        currentValue = 0;
                    if (currentValue >= 1)
                        currentValue = 1;
                    gameData.note.Add(new NoteInfo(songData[i].time, new Vector2(Mathf.Lerp(-noteSpawnWidth, noteSpawnWidth,currentValue), Mathf.Lerp(-noteSpawnHeight, noteSpawnHeight, heightStrenghtIntensity * (songData[i].midrange.strenght - detectLevel)))));
                }
                tempData.midrange.active = true;
            }
            tempData.midrange.active = songData[i].midrange.active;
            if (songData[i].midrange.strenght < noteSpawnLevel)
                tempData.midrange.active = false;

            if (songData[i].upperMidrange.active && !tempData.upperMidrange.active)
            {
                if (songData[i].upperMidrange.strenght >= currentNoteLevel)
                {
                    currentNoteLevel = songData[i].upperMidrange.strenght + noteLevelGain;
                    currentValue += Random.Range(-noteOffsetRange, noteOffsetRange);
                    if (currentValue <= 0)
                        currentValue = 0;
                    if (currentValue >= 1)
                        currentValue = 1;
                    gameData.note.Add(new NoteInfo(songData[i].time, new Vector2(Mathf.Lerp(-noteSpawnWidth, noteSpawnWidth, currentValue), Mathf.Lerp(-noteSpawnHeight, noteSpawnHeight, heightStrenghtIntensity * (songData[i].lowMidrange.strenght - detectLevel)))));
                }
                tempData.upperMidrange.active = true;
            }
            tempData.upperMidrange.active = songData[i].upperMidrange.active;
            if (songData[i].upperMidrange.strenght < noteSpawnLevel)
                tempData.upperMidrange.active = false;


            currentNoteLevel -= noteLevelDrop;
            if (currentNoteLevel <= noteSpawnLevel)
                currentNoteLevel = noteSpawnLevel;
        }

        samplingDisplay.SetActive(false);
        manager.data = gameData;
        StartCoroutine(manager.PlayGame());
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float[] spectrum = new float[1024];
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
        public AudioRecordDataPart subBass = new AudioRecordDataPart();
        public AudioRecordDataPart bass = new AudioRecordDataPart();
        public AudioRecordDataPart lowMidrange = new AudioRecordDataPart();
        public AudioRecordDataPart midrange = new AudioRecordDataPart();
        public AudioRecordDataPart upperMidrange = new AudioRecordDataPart();
        public AudioRecordDataPart presence = new AudioRecordDataPart();
        public AudioRecordDataPart brilliance = new AudioRecordDataPart();
        public float time = 0;
    }

    [System.Serializable]
    public class GameData
    {
        public List<float> bassAttack = new List<float>();
        public List<NoteInfo> note = new List<NoteInfo>();
    }

    public class NoteInfo
    {
        public float time = 0;
        public Vector3 position = new Vector3();

        public NoteInfo(float _time, Vector2 _position)
        {
            time = _time;
            position = new Vector3(_position.x, _position.y);
        }
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

        public AudioRecordDataPart()
        {
            active = false;
            strenght = 0;
        }
    }
}
