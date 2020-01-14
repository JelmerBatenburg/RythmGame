using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float maxMovementWidth;
    public float mouseMoveAmount;
    Vector3 center;

    public LayerMask noteMask;
    public float noteDetectRange;
    public string detectionInput;
    public int score;
    public Text scoreInput;

    public void Start()
    {
        center = transform.position;
    }

    public void Update()
    {
        Move();
        Detect();
    }

    public void Detect()
    {
        if (Input.GetButtonDown(detectionInput))
            foreach (Collider col in Physics.OverlapBox(transform.position, new Vector3(noteDetectRange, noteDetectRange, noteDetectRange * 2f), Quaternion.identity, noteMask))
            {
                score++;
                Destroy(col.gameObject);
                scoreInput.text = "Score: " + score.ToString();
            }
    }

    public void Move()
    {
        float width = Screen.width;
        float mousePos = Input.mousePosition.x;

        float currentPos = 1f / width * mousePos;
        transform.position = center + (Vector3.right * mouseMoveAmount * (currentPos - 0.5f));
        if (transform.position.x >= center.x + maxMovementWidth)
            transform.position = center + (Vector3.right * maxMovementWidth);
        if (transform.position.x <= center.x - maxMovementWidth)
            transform.position = center + (-Vector3.right * maxMovementWidth);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(maxMovementWidth, transform.position.y), new Vector3(0, 2f, 1));
        Gizmos.DrawWireCube(new Vector3(-maxMovementWidth, transform.position.y), new Vector3(0, 2f, 1));
        Gizmos.DrawWireSphere(transform.position, noteDetectRange);
    }
}
