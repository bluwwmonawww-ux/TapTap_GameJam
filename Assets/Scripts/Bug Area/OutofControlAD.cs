
using UnityEngine;

public class OutofControlAD : MonoBehaviour
{
    [Header("AD键失控设置")]
    public bool outcontrol = true;         // 是否启用失控
    public bool forceLeft = true;          // 是否强制向左（A键），false为强制向右（D键）
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 获取玩家的PlayerMovement组件
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            
            if (playerMovement != null)
            {
                if (outcontrol)
                {
                    // 如果outcontrol为真，启用AD键失控
                    if (forceLeft)
                    {
                        playerMovement.SetADOutOfControl(true, false); // 强制向左
                        Debug.Log("玩家AD键失控：强制向左");
                    }
                    else
                    {
                        playerMovement.SetADOutOfControl(false, true); // 强制向右
                        Debug.Log("玩家AD键失控：强制向右");
                    }
                }
                else
                {
                    // 如果outcontrol为假，恢复AD键控制
                    playerMovement.SetADOutOfControl(false, false);
                    Debug.Log("玩家AD键恢复控制");
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 玩家离开时不做任何操作，保持当前状态
            Debug.Log("玩家离开AD键失控区域");
        }
    }
}





























