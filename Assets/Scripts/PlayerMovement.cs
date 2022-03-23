using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animatr;

    //This is the speed limit
    public float maxSpeed; // 4.5 is good max speed
    public float maxJump; // 10 is good / but gravity should be 4 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animatr = GetComponent<Animator>();
    }

    // Update Frame
    // normalized = makes vector size 1. For example, right movement = 1, left movement = -1
    void Update()
    {

        // Jump only once 
        if (Input.GetButtonDown("Jump") && !animatr.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * maxJump, ForceMode2D.Impulse);
            animatr.SetBool("isJumping", true);
        }

        // Prevent slipping like ice. This will stop the speed once key press is releashed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x *0.5f, rigid.velocity.y);
        }

        //Direction: flips the character by detecting the key pressed
        if (Input.GetButtonDown("Horizontal"))
        {
            //Flip on horizontal if key is pressed to left. 
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation isWalking : true => Walk animation / false => Idle
        if (Mathf.Abs( rigid.velocity.x) < 0.3)
        {
            // Speed = 0, not walking
            animatr.SetBool("isWalking", false);
        }
        else {
            // chtr is walking
            animatr.SetBool("isWalking", true);
        }
    }

    // Physic stuff
    void FixedUpdate()
    {
        // Plater Movement by Arrow key
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        if (rigid.velocity.x> maxSpeed) //  Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y );
        }else if (rigid.velocity.x < (-1)*(maxSpeed)) // Left Max Speed (Negative)
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }



        //RayCast : detecting when player lands on the platform
        Debug.DrawRay(rigid.position, Vector3.down, new Color(1,0,0));

        /*RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1);
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.5f)
                Debug.Log("Yeet you hit this : " + rayHit.collider.name);
        }
        */


        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        // Only raycast if character is falling = velocity on y is negative
        if (rigid.velocity.y < 0 && rayHit.collider != null && rayHit.distance < 0.5)
        {
            //if (rayHit.collider != null)
            //{
            //    if (rayHit.distance < 0.5)
            //    {
                    animatr.SetBool("isJumping", false);
            //    }
            //}
        }

    }
        
}
