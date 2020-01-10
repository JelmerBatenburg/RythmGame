using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed;
    public float lifetime;

    public void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Update()
    {
        transform.position += Vector3.back * Time.deltaTime * speed;
    }
}
