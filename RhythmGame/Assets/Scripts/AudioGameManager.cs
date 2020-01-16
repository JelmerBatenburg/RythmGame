using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioGameManager : MonoBehaviour
{
    public GetAudioData.GameData data;
    public AudioSource source;
    public Transform spawnPoint;
    public GameObject note;
    public float spawnWidth;
    float currentTime = 0;
    public float offset;
    public GameObject particle;

    int[] currentIndexes = new int[7];
    public Color[] colors = new Color[7];
    public string[] inputs = new string[7];
    public float inputOffset;
    public float inptRange;
    public LayerMask noteMask;
    public float points;
    public Text pointDisplay;
    public float colorIncrease;
    public float colorToWhiteLerp;

    public IEnumerator PlayGame()
    {
        while (currentTime <= offset)
        {
            CheckSpawn();
            CheckInputs();
            pointDisplay.text = "Points: " + points.ToString();
            currentTime += Time.deltaTime;
            yield return null;
        }

        source.Play();

        while (source.isPlaying)
        {
            CheckSpawn();
            CheckInputs();
            pointDisplay.text = "Points: " + points.ToString();
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    public void CheckInputs()
    {
        for (int i = 0; i < 7; i++)
        {
            Vector3 point = spawnPoint.position + (Vector3.back * inputOffset) + (Vector3.right * Mathf.Lerp(-spawnWidth, spawnWidth, 1f / 7f * i));
            if (Input.GetButton(inputs[i]) && Physics.CheckSphere(point, inptRange, noteMask))
            {
                Note interactedNote = Physics.OverlapSphere(point, inptRange, noteMask)[0].GetComponent<Note>();
                if (!interactedNote.pressed && interactedNote.isString)
                {
                    Destroy(Instantiate(particle, point, particle.transform.rotation), 2f);
                    if (!interactedNote.hasInteracted)
                    {
                        MeshRenderer renderer = interactedNote.GetComponent<MeshRenderer>();
                        Color color = Color.Lerp(renderer.material.GetColor("_EmissionColor"), new Color(1, 1, 1), colorToWhiteLerp);
                        renderer.material.SetColor("_EmissionColor", color * colorIncrease);
                    }
                    interactedNote.Interacted(this);
                }
                else if(!interactedNote.pressed && Input.GetButtonDown(inputs[i]))
                {
                    Destroy(Instantiate(particle, point, particle.transform.rotation), 2f);
                    if (!interactedNote.hasInteracted)
                    {
                        MeshRenderer renderer = interactedNote.GetComponent<MeshRenderer>();
                        Color color = Color.Lerp(renderer.material.GetColor("_EmissionColor"), new Color(1, 1, 1), colorToWhiteLerp);
                        renderer.material.SetColor("_EmissionColor", color * colorIncrease);
                    }
                    interactedNote.Interacted(this);
                }
            }
        }
    }

    public void CheckSpawn()
    {
        for (int i = 0; i < data.types.Count; i++)
        {
            if (currentIndexes[i] <= data.types[i].Count - 1 && currentTime >= data.types[i][currentIndexes[i]].time)
            {
                GameObject g = Instantiate(note, spawnPoint.position + (Vector3.right * Mathf.Lerp(-spawnWidth, spawnWidth, 1f / 7f * i)), Quaternion.identity);
                g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colors[i] * 2f);
                if (data.types[i][currentIndexes[i]].dataType == GetAudioData.GameDataPart.DataType.String)
                {
                    float multiplyValue = g.GetComponent<Note>().speed;
                    g.GetComponent<Note>().isString = true;
                    g.transform.position += transform.forward * data.types[i][currentIndexes[i]].lenght * multiplyValue / 2f;
                    g.transform.localScale += Vector3.forward * data.types[i][currentIndexes[i]].lenght * multiplyValue;
                }
                currentIndexes[i]++;
            }
        }
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < 7; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawWireSphere(spawnPoint.position + (Vector3.right * Mathf.Lerp(-spawnWidth, spawnWidth, 1f / 7f * i)), inptRange);
            Gizmos.DrawWireSphere(spawnPoint.position + (Vector3.back * inputOffset) + (Vector3.right * Mathf.Lerp(-spawnWidth, spawnWidth, 1f / 7f * i)), inptRange);
        }
    }
}
