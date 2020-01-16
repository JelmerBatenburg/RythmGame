using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public bool isString;
    public bool pressed;
    public float normalNoteMultiplier;
    public float stringPointDelay;
    public bool hasInteracted;

    public void Interacted(AudioGameManager manager)
    {
        pressed = true;
        hasInteracted = true;
        manager.points += isString ? 1 : normalNoteMultiplier;
        if (isString)
            StartCoroutine(Delay());
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(stringPointDelay);
        pressed = false;
    }

    public void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Update()
    {
        transform.position += Vector3.back * Time.deltaTime * speed;
    }
}
