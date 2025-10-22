using UnityEngine;

public class SaveMono : MonoBehaviour
{
    public GameObject NewPlayer;
    public GameObject GameObjecttext;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerRespawn()
    {
        //GameObject Newplayer = GameObject.Instantiate(NewPlayer);
        //Newplayer.transform.position = transform.position;
        //gameObject.transform.position = new Vector2(4.05f, 5.8852f);
        //Health = 100;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 调用玩家的Load方法，恢复到存档位置
            var recorder = player.GetComponent<PlayerActionRecorder>();
            if (recorder != null)
            {
                recorder.Load();
            }
        }
        else
        {
            Debug.LogWarning("未找到玩家对象");
        }
    }
    
}
