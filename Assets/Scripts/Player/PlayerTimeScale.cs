using UnityEngine;

public class PlayerTimeScale : MonoBehaviour
{
    [Header("时间缩放设置")]
    public bool BeginTimeScale = false;           // 是否启用时间缩放
    public float maxTimeScale = 3f;        // 最大时间缩放值
    public float minTimeScale = 1f/3f;     // 最小时间缩放值
    public float changeSpeed = 1f;         // 变化速度
    
    private bool isIncreasing = true;      // 是否正在增大
    private float currentTimeScale = 1f;   // 当前时间缩放值
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTimeScale = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (BeginTimeScale)
        {
            UpdateTimeScale();
        }
    }
    
    private void UpdateTimeScale()
    {
        if (isIncreasing)
        {
            // 使用乘法增大时间缩放值
            currentTimeScale *= (1f + changeSpeed * Time.unscaledDeltaTime);
            
            // 检查是否达到最大值
            if (currentTimeScale >= maxTimeScale)
            {
                currentTimeScale = maxTimeScale;
                isIncreasing = false; // 开始减小
            }
        }
        else
        {
            // 使用除法减小时间缩放值
            currentTimeScale /= (1f + changeSpeed * Time.unscaledDeltaTime);
            
            // 检查是否达到最小值
            if (currentTimeScale <= minTimeScale)
            {
                currentTimeScale = minTimeScale;
                isIncreasing = true; // 开始增大
            }
        }
        
        // 应用时间缩放值
        Time.timeScale = currentTimeScale;
    }
    
    // 公共接口：开始时间缩放
    public void StartTimeScale(float maxScale, float minScale)
    {
        BeginTimeScale = true;
        maxTimeScale = maxScale;
        minTimeScale = minScale;
        currentTimeScale = Time.timeScale;
        isIncreasing = true; // 从当前值开始增大
        Debug.Log($"开始时间缩放: 最大值={maxTimeScale}, 最小值={minTimeScale}");
    }
    
    // 公共接口：停止时间缩放
    public void StopTimeScale()
    {
        BeginTimeScale = false;
        // 恢复原始时间缩放
        Time.timeScale = 1f;
        currentTimeScale = 1f;
        Debug.Log("停止时间缩放");
    }
    
    // 公共接口：检查是否正在时间缩放
    public bool IsTimeScaling()
    {
        return BeginTimeScale;
    }
    
    // 公共接口：获取当前时间缩放值
    public float GetCurrentTimeScale()
    {
        return currentTimeScale;
    }
}
