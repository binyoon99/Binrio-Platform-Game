using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public int enemyMovement;
    Animator animatr;

    CapsuleCollider2D collir;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animatr = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collir = GetComponent<CapsuleCollider2D>();
        Invoke("enemyThink", 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(enemyMovement,rigid.velocity.y);
        // enemyThink();


        //Check if enemy fall
        Vector2 frontVectorCheck = new Vector2(rigid.position.x + enemyMovement*0.2f, rigid.position.y);
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVectorCheck, Vector3.down, 1, LayerMask.GetMask("Platform"));

        // Only raycast if character is falling = velocity on y is negative
        if (rayHit.collider == null )
        {
            Debug.Log("Monster will fall");
            //if mob is about to fall change enemyMovement
            enemyMovement *= -1;

            flipMob();
            // if mob change direction mob think again
            CancelInvoke();
            Invoke("enemyThink", 2);
        }


    }
    // 
    void enemyThink()
    {
        enemyMovement = Random.Range(-1, 2);

        // This little thing is very stupid it will fall off :/
        // we need ray but like D: i dont wanna code that
        // imma just copy paste  from playermovement


        //if (enemyMovement == 0)
        //{
        //    animatr.SetInteger("walkingSpeed", 0);

        //}else if (enemyMovement == -1)
        //{
        //    animatr.SetInteger("walkingSpeed", -1);
        //}else if (enemyMovement == 1)
        //{
        //    animatr.SetInteger("walkingSpeed", 1);
        //}
        animatr.SetInteger("walkingSpeed", enemyMovement);

        flipMob();


        float nextThinkingTime = Random.Range(2f, 4f);
        // 재귀함수 : 5초 뒤에 enemyThink 실행
        Invoke("enemyThink", nextThinkingTime);

    }

    void flipMob()
    {
        if (enemyMovement == -1)
        {
            spriteRenderer.flipX = false;
        }
        else if (enemyMovement == 1)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void onDamaged()
    {
        //mob goes trsnsparent 
        spriteRenderer.color = new Color(1, 1, 1,0.4f);

        //flips y
        spriteRenderer.flipY = true;

        //disable collider so mob falls down
        collir.enabled = false;

        // mob also jump before it dies
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Then WE KILL THE MOB
        Invoke("killMob", 4);

    }
    void killMob()
    {
        gameObject.SetActive(false);
    }
}
