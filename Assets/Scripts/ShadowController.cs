using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [Header("影子设置")]
    [SerializeField] private PlayerActionRecorder playerRecorder;
    [SerializeField] private float timeOffset = 2f;
    [SerializeField] private Color shadowColor = new Color(1f, 1f, 1f, 0.5f); 
    
    [Header("重叠效果")]
    [SerializeField] private float overlapThreshold = 0.5f; 
    [SerializeField] private GameObject overlapEffect; 
    [SerializeField] private Color overlapColor = Color.cyan; 
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
        if (shadowSprite != null)
        {
            shadowSprite.color = shadowColor;
        }
        if (shadowRigidBody != null)
        {
            shadowRigidBody.isKinematic = true;
        }
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
        PlayerActionFrame targetFrame = playerRecorder.GetActionFrame(timeOffset);
        if (targetFrame != null)
        {
            targetPosition = targetFrame.position;
            transform.rotation = targetFrame.rotation;
            UpdateAnimations(targetFrame);
        }
        CheckOverlap();
    }
    
    void FixedUpdate()
    {
        if (targetPosition != Vector3.zero)
        {
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
        if (shadowSprite != null)
        {
            shadowSprite.color = overlapColor;
        }
        
        if (overlapEffect != null)
        {
            overlapEffect.SetActive(true);
        }
        Debug.Log("影子与玩家重叠！");
        
        // 重叠时死亡
        if (enableOverlapPower && playerController != null)
        {
            Debug.Log("触发重叠特殊能力！");
        }
    }
    
    private void OnOverlapEnd()
    {
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
    
    public void SetShadowColor(Color color)
    {
        shadowColor = color;
        if (shadowSprite != null)
        {
            shadowSprite.color = shadowColor;
        }
    }
   
    public void SetTimeOffset(float offset)
    {
        timeOffset = offset;
    }
}

