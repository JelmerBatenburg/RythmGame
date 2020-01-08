using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffect : MonoBehaviour
{
    public GameObject triangle;
    public Transform triangleSpawnPos;
    public float traingleSpeed;
    public float traingleSpawnSpeed;
    public float traingleLifetime;
    public float rotateSpeed;
    public List<GameObject> activeVisuals;
    public float bassValue;
    public float bassDropSpeed;
    public float bassMinValue;
    public Color lineColor;

    public void Start()
    {
        StartCoroutine(TriangleSpawner());
    }

    public void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        foreach (GameObject part in activeVisuals)
            part.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", lineColor * bassValue);
        bassValue = Mathf.Lerp(bassValue, bassMinValue, Time.deltaTime * bassDropSpeed);
    }

    public IEnumerator TriangleSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(traingleSpawnSpeed);
            StartCoroutine(TraingelSpawn());
        }
    }

    public IEnumerator TraingelSpawn()
    {
        float time = 0;
        GameObject g = Instantiate(triangle, triangleSpawnPos.position, triangleSpawnPos.rotation, transform);
        activeVisuals.Add(g);

        while(time <= traingleLifetime)
        {
            time += Time.deltaTime;
            g.transform.position += Vector3.forward * Time.deltaTime * -traingleSpeed;
            yield return null;
        }
        activeVisuals.Remove(g);
        Destroy(g);
    }
}
