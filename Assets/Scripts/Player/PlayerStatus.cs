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
        var items = Object.FindObjectsByType<ItemMagnet>(FindObjectsSortMode.None);

        foreach (var item in items)
        {
            item.ResetItem();
        }
        //PlayerRespawn();
        Health = 100;
        StartCoroutine(PlayerDying());
        Debug.Log("Dead");
        
    }
    public void PlayerRespawn()
    {
        var recorder = gameObject.GetComponent<PlayerActionRecorder>();
        if (recorder != null)
        {
            recorder.Load(); 
        }
        else
        {
            Debug.LogWarning("Œ¥’“µΩ PlayerActionRecorder ");
        }
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
