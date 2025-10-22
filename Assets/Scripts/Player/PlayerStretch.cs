using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStretch : MonoBehaviour
{
    [Header("拉伸设置")]
    public float stretchSpeed = 2f;        // 拉伸速度
    public float maxStretchRatio = 2f;     // 最大拉伸比例（宽度倍数）
    public float minStretchRatio = 0.5f;   // 最小拉伸比例（高度倍数）
    
    [Header("输入设置")]
    public InputActionReference stretchAction; // J键输入动作
    
    // 内部变量
    private Transform playerTransform;
    private Vector3 originalScale;
    private Vector3 currentTargetScale;
    private bool isStretching = false;
    private bool stretchInputPressed = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 获取玩家Transform
        playerTransform = transform;
        originalScale = playerTransform.localScale;
        currentTargetScale = originalScale;
        
        // 设置输入动作
        if (stretchAction != null)
        {
            stretchAction.action.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateStretch();
    }
    
    private void HandleInput()
    {
        if (stretchAction != null)
        {
            // 检测J键输入
            bool inputThisFrame = stretchAction.action.ReadValue<float>() > 0.5f;
            
            if (inputThisFrame && !stretchInputPressed)
            {
                // J键刚按下
                StartStretching();
                stretchInputPressed = true;
            }
            else if (!inputThisFrame && stretchInputPressed)
            {
                // J键刚松开
                StopStretching();
                stretchInputPressed = false;
            }
        }
    }
    
    private void UpdateStretch()
    {
        if (isStretching)
        {
            // 逐渐拉伸到目标比例，保持原有的朝向（scale.x的正负值）
            Vector3 currentScale = playerTransform.localScale;
            float currentOrientation = Mathf.Sign(currentScale.x); // 获取当前朝向（正负）
            
            Vector3 targetScale = new Vector3(
                Mathf.Abs(originalScale.x) * maxStretchRatio * currentOrientation,  // 宽度增加，保持朝向
                originalScale.y * minStretchRatio,  // 高度减少
                originalScale.z                     // 深度不变
            );
            currentTargetScale = targetScale;
        }
        else
        {
            // 逐渐恢复到原始比例，保持当前的朝向
            Vector3 currentScale = playerTransform.localScale;
            float currentOrientation = Mathf.Sign(currentScale.x); // 获取当前朝向（正负）
            
            Vector3 targetScale = new Vector3(
                Mathf.Abs(originalScale.x) * currentOrientation,  // 恢复原始宽度，保持朝向
                originalScale.y,  // 恢复原始高度
                originalScale.z   // 深度不变
            );
            currentTargetScale = targetScale;
        }
        
        // 平滑过渡到目标缩放
        playerTransform.localScale = Vector3.Lerp(
            playerTransform.localScale, 
            currentTargetScale, 
            stretchSpeed * Time.deltaTime
        );
    }
    
    // 开始拉伸
    private void StartStretching()
    {
        isStretching = true;
        Debug.Log("开始拉伸");
    }
    
    // 停止拉伸
    private void StopStretching()
    {
        isStretching = false;
        Debug.Log("停止拉伸，开始恢复");
    }
    
    // 公共接口：手动设置拉伸状态
    public void SetStretching(bool stretching)
    {
        isStretching = stretching;
    }
    
    // 公共接口：检查是否正在拉伸
    public bool IsStretching()
    {
        return isStretching;
    }
    
    // 公共接口：立即重置到原始大小，保持当前朝向
    public void ResetToOriginalScale()
    {
        Vector3 currentScale = playerTransform.localScale;
        float currentOrientation = Mathf.Sign(currentScale.x); // 获取当前朝向（正负）
        
        Vector3 resetScale = new Vector3(
            Mathf.Abs(originalScale.x) * currentOrientation,  // 保持朝向
            originalScale.y,
            originalScale.z
        );
        
        playerTransform.localScale = resetScale;
        currentTargetScale = resetScale;
        isStretching = false;
    }
    
    // 公共接口：获取当前拉伸比例（绝对值）
    public float GetCurrentStretchRatio()
    {
        return Mathf.Abs(playerTransform.localScale.x) / Mathf.Abs(originalScale.x);
    }
    
    void OnDestroy()
    {
        // 清理输入动作
        if (stretchAction != null)
        {
            stretchAction.action.Disable();
        }
    }
}
