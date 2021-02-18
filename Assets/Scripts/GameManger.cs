using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour //Manage Point and Stage
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIpoint;
    public Text UIstage;
    public GameObject UIRestartBtn;

    private void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        if(stageIndex < Stages.Length - 1) //Change Stage
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            Respawn();

            UIstage.text = "Stage " + (stageIndex + 1);
        }
        else //Game Clear
        {
            //Player Time Lock
            Freeze(); 

            //Result UI
            Debug.Log("Game Clear.");

            //Restart Button
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }

        //Point Manage
        totalPoint += stagePoint;
        stagePoint = 0;
    }
    
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = Color.black;
        }
        else //Player Die
        {
            //All Health UI Off
            UIhealth[0].color = Color.black;

            //Die Effect
            player.OnDie();

            //Result UI
            Debug.Log("You Died.");

            //Retry Button UI
            UIRestartBtn.SetActive(true);

            Invoke("Freeze", 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")   //Respawn
        {
            //Respawn
            if (health > 1)
                Respawn();

            //Health Down
            HealthDown();
        }
    }
    void Respawn()
    {
        player.transform.position = new Vector2(0, 0);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    void Freeze()
    {
        Time.timeScale = 0; //완주, 실패하게 되면 timeScale = 0 으로 시간을 멈춰둠.
    }
}