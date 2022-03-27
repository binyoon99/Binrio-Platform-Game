using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int healthPoint;

    public PlayerMovement player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restartButton;
    // Start is called before the first frame update
    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        if (stageIndex< Stages.Length-1)
        {
            //Change stage
            Stages[stageIndex].SetActive(false);

            stageIndex++;

            Stages[stageIndex].SetActive(true);
            playerReposition();
            UIStage.text = "STAGE" + (stageIndex+1) ;
        }
        else
        {
            // Game clear!

            //Player control lock
            Time.timeScale = 0;
            //result ui
            Debug.Log("Game clear");
            //restart button ui

            Text btnText = restartButton.GetComponentInChildren<Text>();
            btnText.text = "Game Clear :D";
            restartButton.SetActive(true);

        }
        

        // Point Calculation 
        totalPoint += stagePoint;
        stagePoint = 0;
    }
    // Update is called once per frame


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            if (healthPoint > 1)
            {

                playerReposition();
                stagePoint -= 50;
            //collision.attachedRigidbody.velocity = Vector2.zero;
            ////player position reset
            //collision.transform.position = new Vector3(-2, 1, -1);
            //player die player lose 50 points;

            }
            healthLost();


           
        }
    }
    public void healthLost()
    {
      
        if (healthPoint > 1) {
            healthPoint--;
            stagePoint -= 50;
            UIhealth[healthPoint].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //player die effect
            player.onDie();

            // all points off

            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //reseut ui
            restartButton.SetActive(true); restartButton.SetActive(true);
            Debug.Log("YOU DIED");
            //retry button ui
        }
       

    }

    public void playerReposition()
    {
        player.transform.position = new Vector3(-3, 1, -1);
        player.VelocityZero();
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        
    }
}
