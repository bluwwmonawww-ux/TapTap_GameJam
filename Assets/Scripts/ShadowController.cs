using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [Header("影子设置")]
    [SerializeField] private PlayerActionRecorder playerRecorder;
    [SerializeField] private float timeOffset = 2f; // 延迟2秒
    [SerializeField] private Color shadowColor = new Color(1f, 1f, 1f, 0.5f); // 半透明
    
    [Header("重叠效果")]
    [SerializeField] private float overlapThreshold = 0.5f; // 重叠检测距离
    [SerializeField] private GameObject overlapEffect; // 重叠时的特效
    [SerializeField] private Color overlapColor = Color.cyan; // 重叠时的颜色
    [SerializeField] private bool enableOverlapPower = true;
    
    [Header("组件")]
    [SerializeField] private Rigidbody2D shadowRigidBody;
    [SerializeField] private Animator shadowAnimator;
    [SerializeField] private SpriteRenderer shadowSprite;
    
    private Vector3 targetPosition;
    private bool isOverlapping = false;
    private PlayerController playerController;
    
    void Start()
    {
        // 设置影子外观
        if (shadowSprite != null)
        {
            shadowSprite.color = shadowColor;
        }
        
        // 禁用物理交互
        if (shadowRigidBody != null)
        {
            shadowRigidBody.isKinematic = true;
        }
        
        // 找到玩家的记录器
        if (playerRecorder == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerRecorder = player.GetComponent<PlayerActionRecorder>();
                playerController = player.GetComponent<PlayerController>();
            }
        }
    }
    
    void Update()
    {
        if (playerRecorder == null) return;
        
        // 获取2秒前的动作数据
        PlayerActionFrame targetFrame = playerRecorder.GetActionFrame(timeOffset);
        
        if (targetFrame != null)
        {
            // 应用位置和旋转
            targetPosition = targetFrame.position;
            transform.rotation = targetFrame.rotation;
            
            // 更新动画参数
            UpdateAnimations(targetFrame);
        }
        
        // 检测是否与玩家重叠
        CheckOverlap();
    }
    
    void FixedUpdate()
    {
        if (targetPosition != Vector3.zero)
        {
            // 平滑移动到目标位置
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 10f);
            if (shadowRigidBody != null)
            {
                shadowRigidBody.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }
        }
    }
    
    private void UpdateAnimations(PlayerActionFrame frame)
    {
        if (shadowAnimator == null) return;
        
        // 设置动画参数
        shadowAnimator.SetFloat("Speed", Mathf.Abs(frame.moveDir.x));
        shadowAnimator.SetBool("IsGrounded", frame.isGrounded);
        shadowAnimator.SetBool("IsJumping", frame.isJumping);
        shadowAnimator.SetFloat("VelocityY", frame.velocity.y);
    }
    
    private void CheckOverlap()
    {
        if (playerRecorder == null) return;
        
        float distance = Vector3.Distance(transform.position, playerRecorder.transform.position);
        bool currentlyOverlapping = distance < overlapThreshold;
        
        // 重叠状态改变时的处理
        if (currentlyOverlapping && !isOverlapping)
        {
            OnOverlapStart();
        }
        else if (!currentlyOverlapping && isOverlapping)
        {
            OnOverlapEnd();
        }
        
        isOverlapping = currentlyOverlapping;
    }
    
    private void OnOverlapStart()
    {
        // 重叠开始时的效果
        if (shadowSprite != null)
        {
            shadowSprite.color = overlapColor;
        }
        
        if (overlapEffect != null)
        {
            overlapEffect.SetActive(true);
        }
        
        // 可以添加音效、屏幕震动等
        Debug.Log("影子与玩家重叠！");
        
        // 重叠时获得特殊能力
        if (enableOverlapPower && playerController != null)
        {
            // 这里可以添加各种特殊效果
            // 例如：伤害翻倍、无敌状态、特殊技能等
            Debug.Log("触发重叠特殊能力！");
        }
    }
    
    private void OnOverlapEnd()
    {
        // 重叠结束时的效果
        if (shadowSprite != null)
        {
            shadowSprite.color = shadowColor;
        }
        
        if (overlapEffect != null)
        {
            overlapEffect.SetActive(false);
        }
        
        Debug.Log("重叠结束");
    }
    
    /// <summary>
    /// 设置影子颜色
    /// </summary>
    public void SetShadowColor(Color color)
    {
        shadowColor = color;
        if (shadowSprite != null)
        {
            shadowSprite.color = shadowColor;
        }
    }
    
    /// <summary>
    /// 设置时间偏移
    /// </summary>
    public void SetTimeOffset(float offset)
    {
        timeOffset = offset;
    }
}

