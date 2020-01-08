using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEffect : MonoBehaviour
{
    public List<GameObject> activeVisuals;
    public float bassValue;
    public float bassDropSpeed;
    public float bassMinValue;
    public Color lineColor;

    public void Update()
    {
        foreach (GameObject part in activeVisuals)
            part.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", lineColor * bassValue);
        bassValue = Mathf.Lerp(bassValue, bassMinValue, Time.deltaTime * bassDropSpeed);
    }
}
