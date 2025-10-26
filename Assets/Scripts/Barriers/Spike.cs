using UnityEngine;

public class Spike : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private bool playerInArea;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.CompareTag("Player"))
        {
            playerStatus = other.GetComponent<PlayerStatus>();


            playerInArea = true;

            playerStatus.Health -= 101;


        }
    }
   
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    // 检查离开的对象是否是玩家
    //    if (other.CompareTag("Player") && playerInArea)
    //    {

    //        // 恢复原始力
    //        playerMovement.InverseAD = false;


    //        playerInArea = false;
    //        playerMovement = null;
    //    }
    //}
}
