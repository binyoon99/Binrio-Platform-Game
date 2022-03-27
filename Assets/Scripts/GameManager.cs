using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int healthPoint;

    public PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    { 
        
    }

    public void NextStage()
    {
        stageIndex++;
        totalPoint += stagePoint;
        stagePoint = 0;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            if (healthPoint > 1)
            {
    collision.attachedRigidbody.velocity = Vector2.zero;
            //player position reset
            collision.transform.position = new Vector3(-2, 1, -1);
            //player die player lose 50 points;
      
            }
            healthLost();


           
        }
    }
    public void healthLost()
    {
      
        if (healthPoint > 0) {
            healthPoint--;
            stagePoint -= 50;
        }
        else
        {
            //player die effect
            player.onDie();
            //reseut ui

            Debug.Log("YOU DIED");
            //retry button ui
        }
       

    }

}
