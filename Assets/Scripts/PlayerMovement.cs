using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animatr;
    CapsuleCollider2D collir;

    //This is the speed limit
    public float maxSpeed; // 4.5 is good max speed
    public float maxJump; // 10 is good / but gravity should be 4

    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioCoin;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animatr = GetComponent<Animator>();
        collir = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    void playSound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "COIN":
                audioSource.clip = audioCoin;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;

        }
        audioSource.Play();
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
            playSound("JUMP");
        }

        // Prevent slipping like ice. This will stop the speed once key press is releashed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x *0.5f, rigid.velocity.y);
        }

        //Direction: flips the character by detecting the key pressed
        if (Input.GetButton("Horizontal"))
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
        rigid.AddForce(Vector2.right * h*2, ForceMode2D.Impulse);
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

    //Coins

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            playSound("COIN");
            //gains point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }else if (isGold)
            {
                gameManager.stagePoint += 300;
            }
            
            // deactive coin
            collision.gameObject.SetActive(false);

            

        }else if (collision.gameObject.tag == "Finish")
        {
            //stage is finished ~~ next stage plz
            playSound("FINISH");
            gameManager.NextStage();

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // If Player is on top of mob + force is going down (he is falling)
            // then player is killing mob like mario
            // rigid.velocity.y < 0  = player is falling

            //transform.position.y> collision.transform.position.y= player is on top of mob
            if (rigid.velocity.y < 0 && transform.position.y> collision.transform.position.y)
            {
                Debug.Log("Attacking mob");
                attacking(collision.transform);
                playSound("ATTACK");
            }
            else
            {
                Debug.Log("You got hit by mob");
                playSound("DAMAGED");
                onDamaged(collision.transform.position);
            }
            
        }else if (collision.gameObject.tag == "Spike")
        {
            Debug.Log("You got hit by spike");
            playSound("DAMAGED");
            onDamaged(collision.transform.position);
        }
        
    }

    void onDamaged(Vector2 enemyPos)
    {

        // health --
        gameManager.healthLost();

        //layer11 = playerDamaged
        gameObject.layer = 11;

        //If damaged player become transparent for 1sec
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //If damaged, player is pushed back
        int pushedBack = transform.position.x - enemyPos.x > 0 ? 1 : -1;
        //if (transform.position.x - enemyPos.x > 0)
        //{
        //    pushedBack = 1;
        //}else { pushedBack = -1; }

        rigid.AddForce(new Vector2(pushedBack, 1)*10, ForceMode2D.Impulse);

        //wait 2second and player is off from being invincible 
        Invoke("offDamaged", 2);

        // Get hit damage animation,
        animatr.SetTrigger("isDamaged");
    }

    void offDamaged()
    {
        //layer10 = plyaer
        gameObject.layer = 10;

        //No longer transparent
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void attacking(Transform enemyTrans)

    {

        //points
        gameManager.stagePoint += 100;

        //pushedback

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //mob dies
        EnemyMove enemyMove = enemyTrans.GetComponent<EnemyMove>();
        enemyMove.onDamaged();
    }

    public void onDie()
    {
        playSound("DIE");

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //flips y
        spriteRenderer.flipY = true;

        //disable collider so mob falls down
        collir.enabled = false;

        // mob also jump before it dies
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
