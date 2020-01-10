using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float maxMovementWidth;
    public Vector3 force;
    public float movementSpeed;
    public float moveHeight;
    public float jumpHeight;
    public float gravity;
    public bool onGround;
    public float verticalLerp;
    public float horizontalLerp;
    public float fallingModifier;

    public void Update()
    {
        Move();
    }

    public void Move()
    {
        force += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * movementSpeed;

        if (force.x > 0 && transform.position.x >= maxMovementWidth || force.x < 0 && transform.position.x <= -maxMovementWidth)
            force.x = 0;

        if (!onGround && transform.position.y <= moveHeight && force.y < 0)
        {
            force.y = 0;
            onGround = true;
        }
        else if (!onGround)
            force += Vector3.down * gravity * Time.deltaTime * ((force.y <= 0) ? fallingModifier : 1f);
        else if (Input.GetButtonDown("Jump"))
        {
            onGround = false;
            force += Vector3.up * jumpHeight;
        }

        force.y = Mathf.Lerp(force.y, 0, verticalLerp * Time.deltaTime);
        force.x = Mathf.Lerp(force.x, 0, horizontalLerp * Time.deltaTime);

        transform.position += force * Time.deltaTime;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.up * moveHeight, Vector3.right * maxMovementWidth * 2f + transform.forward);
        Gizmos.DrawWireCube(new Vector3(maxMovementWidth, moveHeight + 1f), new Vector3(0, 2f, 1));
        Gizmos.DrawWireCube(new Vector3(-maxMovementWidth, moveHeight + 1f), new Vector3(0, 2f, 1));
    }
}
