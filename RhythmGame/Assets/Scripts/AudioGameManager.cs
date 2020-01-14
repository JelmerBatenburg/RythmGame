using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGameManager : MonoBehaviour
{
    public GetAudioData.GameData data;
    public AudioSource source;
    public Transform spawnPoint;
    public GameObject note;
    float currentTime = 0;
    public float offset;
    int currentAttackIndex = 0;
    int currentNoteIndex = 0;

    public Transform[] spawnPoints;
    public Transform player;
    public GameObject explosion;

    public IEnumerator PlayGame()
    {
        Debug.Log(data.bassAttack.Count);
        while (currentTime <= offset)
        {
            CheckSpawn();
            currentTime += Time.deltaTime;
            yield return null;
        }

        source.Play();

        while (source.isPlaying)
        {
            CheckSpawn();

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    public void CheckSpawn()
    {
        if (currentTime >= data.bassAttack[currentAttackIndex])
        {
            StartCoroutine(Rocket(player.position));
            currentAttackIndex++;
        }

        if (currentTime >= data.note[currentNoteIndex].time)
        {
            Instantiate(note, spawnPoint.position + data.note[currentNoteIndex].position, Quaternion.identity);
            currentNoteIndex++;
        }
    }

    public IEnumerator Rocket(Vector3 position)
    {
        spawnPoints[Random.Range(0, spawnPoints.Length)].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(offset - 0.1f);
        Destroy(Instantiate(explosion, position, Quaternion.identity), 0.5f);
    }
}
