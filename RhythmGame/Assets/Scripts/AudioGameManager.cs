using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGameManager : MonoBehaviour
{
    public GetAudioData.GameData data;
    public AudioSource source;
    public ParticleSystem explosion;
    public Transform spawnPoint;
    public GameObject note;
    float currentTime = 0;
    public float StartOffset;
    public float noteSpawnOffset;
    
    public IEnumerator PlayGame()
    {
        source.Play();
        int currentAttackIndex = 0;
        int currentNoteIndex = 0;

        while (source.isPlaying)
        {
            if(currentTime >= data.bassAttack[currentAttackIndex])
            {
                explosion.Play();
                currentAttackIndex++;
            }

            if (currentTime >= data.note[currentNoteIndex].time)
            {
                Instantiate(note, spawnPoint.position + data.note[currentNoteIndex].position, Quaternion.identity);
                currentNoteIndex++;
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
