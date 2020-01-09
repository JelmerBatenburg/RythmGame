using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGameManager : MonoBehaviour
{
    public GetAudioData.GameData data;
    public AudioSource source;
    public ParticleSystem explosion;
    
    public IEnumerator PlayGame()
    {
        source.Play();
        float currentTime = 0;
        int currentAttackIndex = 0;

        while (source.isPlaying)
        {
            if(currentTime >= data.bassAttack[currentAttackIndex])
            {
                explosion.Play();
                currentAttackIndex++;
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
