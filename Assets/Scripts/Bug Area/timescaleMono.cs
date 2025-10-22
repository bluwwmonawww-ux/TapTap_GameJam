using UnityEngine;

public class timescaleMono : MonoBehaviour
{
    [Header("时间缩放控制设置")]
    public bool enableTimeScale = true;    // 是否启用时间缩放
    public float maxTimeScale = 3f;        // 最大时间缩放值
    public float minTimeScale = 1f/3f;     // 最小时间缩放值
    
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
            // 获取玩家的PlayerTimeScale组件
            PlayerTimeScale playerTimeScale = collision.GetComponent<PlayerTimeScale>();
            
            if (playerTimeScale != null)
            {
                if (enableTimeScale)
                {
                    // 如果bool为真，开始时间缩放
                    playerTimeScale.StartTimeScale(maxTimeScale, minTimeScale);
                    Debug.Log($"玩家进入时间缩放区域: 最大值={maxTimeScale}, 最小值={minTimeScale}");
                }
                else
                {
                    // 如果bool为假，停止时间缩放
                    playerTimeScale.StopTimeScale();
                    Debug.Log("玩家进入停止时间缩放区域");
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 玩家离开时不做任何操作，保持当前状态
            Debug.Log("玩家离开时间缩放区域");
        }
    }
}
