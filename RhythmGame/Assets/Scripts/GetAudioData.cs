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
    public float lineCombineTime;

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
        for (int i = 0; i < 7; i++)
            gameData.types.Add(new List<GameDataPart>());

        for (int i = 0; i < songData.Count; i++)
        {
            if (songData[i].subBass.active)
                gameData.types[1].Add(new GameDataPart(songData[i].time));

            if (songData[i].bass.active)
                gameData.types[1].Add(new GameDataPart(songData[i].time));

            if (songData[i].lowMidrange.active)
                gameData.types[2].Add(new GameDataPart(songData[i].time));

            if (songData[i].midrange.active)
                gameData.types[3].Add(new GameDataPart(songData[i].time));

            if (songData[i].upperMidrange.active)
                gameData.types[4].Add(new GameDataPart(songData[i].time));

            if (songData[i].presence.active)
                gameData.types[5].Add(new GameDataPart(songData[i].time));

            if (songData[i].brilliance.active)
                gameData.types[6].Add(new GameDataPart(songData[i].time));
        }

        samplingDisplay.SetActive(false);
        manager.data = FilterLines(gameData);
        StartCoroutine(manager.PlayGame());
    }

    public GameData FilterLines(GameData data)
    {
        GameData newData = new GameData();
        for (int i = 0; i < 7; i++)
            newData.types.Add(new List<GameDataPart>());

        for (int i = 0; i < data.types.Count; i++)
        {
            foreach (GameDataPart part in data.types[i])
            {
                if (newData.types[i].Count == 0 || part.time - (newData.types[i][newData.types[i].Count - 1].time + newData.types[i][newData.types[i].Count - 1].lenght) >= lineCombineTime)
                    newData.types[i].Add(part);
                else
                {
                    GameDataPart lastPart = newData.types[i][newData.types[i].Count - 1];
                    if (lastPart.dataType == GameDataPart.DataType.String && part.time - (lastPart.time + lastPart.lenght) <= lineCombineTime)
                        newData.types[i][newData.types[i].Count - 1].lenght = part.time - lastPart.time;
                    else if (part.time - lastPart.time <= lineCombineTime)
                    {
                        newData.types[i][newData.types[i].Count - 1].dataType = GameDataPart.DataType.String;
                        newData.types[i][newData.types[i].Count - 1].lenght = part.time - lastPart.time;
                    }
                }
            }
        }
        return newData;
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
        public List<List<GameDataPart>> types = new List<List<GameDataPart>>(7);
    }

    public class GameDataPart
    {
        public enum DataType { Note, String}
        public DataType dataType;
        public float time;
        public float lenght;
        
        public  GameDataPart(float _time)
        {
            dataType = DataType.Note;
            time = _time;
        }

        public GameDataPart(float _time, float _Lenght)
        {
            dataType = DataType.Note;
            time = _time;
            lenght = _Lenght;
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
