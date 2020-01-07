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

    public void Start()
    {
        StartCoroutine(TriangleSpawner());
        bassValue = Mathf.Lerp(bassValue, 0, Time.deltaTime * 2f);
        foreach(GameObject part in activeVisuals)
            part.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0,0,0) * bassValue);
    }

    public void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
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
