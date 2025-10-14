using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float Health;
    public GameObject NewPlayer;
    void Start()
    {
        Health = 100;
    }

    void Update()
    {
        if (Health <= 0) {

            PlayerDie();
        }
    }
    public void PlayerDie()
    {

        //PlayerRespawn();
        Health = 100;
        StartCoroutine(PlayerDying());
        Debug.Log("Dead");
        
    }
    public void PlayerRespawn()
    {


        //GameObject Newplayer = GameObject.Instantiate(NewPlayer);
        //Newplayer.transform.position = new Vector2(4.05f, 5.8852f);
        gameObject.transform.position = GameObject.FindGameObjectWithTag("Save").transform.position;
        Health = 100;

    }
    private IEnumerator PlayerDying(float i=3)
    {
        GameObject saveobject = GameObject.FindGameObjectWithTag("Save");
        TextMeshProUGUI Timingtext=saveobject.GetComponent<SaveMono>().GameObjecttext.GetComponent<TextMeshProUGUI>();
        Timingtext.text = "5";
        gameObject.GetComponent<PlayerMovement>().InhibitInput = true;
        while (i >0)
        {
            
                Timingtext.text = ((int)i).ToString();
            
                gameObject.transform.position = new Vector2(100, 58852f);
            i-=Time.deltaTime;
            yield return null;

        }
        Timingtext.text = "";
        PlayerRespawn();
        gameObject.GetComponent<PlayerMovement>().InhibitInput = false;
    }
}
