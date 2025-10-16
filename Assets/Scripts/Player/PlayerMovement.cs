using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float Acceleration ;
    //[SerializeField] private float acceleration = 10f;
    [SerializeField] private float maxSpeed ;

    [Header("跳跃参数")]
    [SerializeField] private float jumpForce ;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer = 1; // 默认层
    [Header("空中控制力是否减半")]
    [SerializeField] private bool HalfForce;
    [Header("物理参数")]
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private bool usePhysics = true;

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform graphics;

    
    [Header("输入设置")]
    [SerializeField] private InputAction playerInput;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    // 内部变量
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isFacingRight = true;
    [SerializeField] private Quaternion originalPlayerRotation;

    // 状态变量
    public bool InhibitInput;
    private Animator animator;
    private bool hasAnimator;
    public Vector2 Force;
    public Rigidbody2D rigidbody2D;
    public bool playerInUnomalyArea = false;
    public bool FlipingPlayer;
    public bool RestoringRotation;
    public  bool isGrounded;
    private bool jumpPressed;
    [SerializeField] public  bool InverseAD;
    private Vector2 PlatformVelocity;
    private Vector2 LastFPlatformVelocity;
    void Start()
    {
        originalPlayerRotation = Quaternion.Euler(0, 0, 0);

        // 自动获取组件
        rigidbody2D = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (graphics == null)
            graphics = transform.Find("Graphics");

        // 设置输入动作
        SetupInputActions();

        animator = GetComponent<Animator>();
        hasAnimator = animator != null;
        currentSpeed = 0f;
    }

    void Update()
    {
        CheckRotation();
        GetInput();
        
        HandleAnimation();
        FlipCharacter();
       
    }

    void FixedUpdate()
    {

        PlatformVelocity = Vector2.zero;
        CheckGrounded();
        if (Force != Vector2.zero && rb != null)
        {
            rb.AddForce(Force);
        }

        // 处理跳跃
        if (jumpPressed && isGrounded)
        {
            Debug.Log("jumpPressedjumpPressedjumpPressedjumpPressed");
            Jump();
            jumpPressed = false;
        }

        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
        }
        LastFPlatformVelocity = PlatformVelocity;
    }

    // 设置输入动作
    void SetupInputActions()
    {
        

        // 启用输入动作
        if (jumpAction != null)
        {
            jumpAction.action.Enable();
        }

        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    // 获取输入
    void GetInput()
    {
        
        if (!InhibitInput)
        {

            moveInput = moveAction.action.ReadValue<Vector2>();
            if (InverseAD)
            {
                moveInput = new Vector2(-moveInput.x, moveInput.y);
            }

            float jumpValue = jumpAction.action.ReadValue<float>();
            if (jumpValue > 0.5f && !jumpPressed)
            {
                jumpPressed = true;
            }
            else
            {
                jumpPressed = false ;
            }
        }
        else
        {
            moveInput = Vector2.zero;
            jumpPressed = false;
        }
    }

    // 检查是否在地面上
    void CheckGrounded()
    {
        Vector2 rayStart = (Vector2)transform.position - transform.up * new Vector2(0, GetComponent<Collider2D>().bounds.extents.y + 0.01f);

        // 使用 RaycastAll 检测所有碰撞体
        RaycastHit2D[] allHits = Physics2D.RaycastAll(rayStart, -transform.up, groundCheckDistance, groundLayer);

        bool foundGround = false;

        // 遍历所有命中的碰撞体
        foreach (RaycastHit2D hit in allHits)
        {
            if (hit.collider != null)
            {

                if (hit.collider.CompareTag("Ground"))
                {

                    foundGround = true;
                    if (hit.collider.TryGetComponent<MovePlatform>(out MovePlatform moveplatform))
                    {
                        rb.linearVelocity -= LastFPlatformVelocity;
                        rb.linearVelocity += (Vector2)moveplatform.CalculateMoveSpeed();
                        PlatformVelocity = (Vector2)moveplatform.CalculateMoveSpeed();
                    }
                    break; 
                }
            }
        }

        isGrounded = foundGround;

        // 调试可视化
        Debug.DrawRay(rayStart, -transform.up * groundCheckDistance, isGrounded ? Color.green : Color.red, 0.1f);
        //if (isGrounded)
        //{
        //    gameObject.GetComponent<HeightTrackMono>().EndJumpTracking();
        //}
    }

    // 跳跃方法
    void Jump()
    {
        if (rb != null && isGrounded)
        {
            // 总是使用玩家的向上方向作为跳跃方向
            //gameObject.GetComponent<HeightTrackMono>().StartJumpTracking();
            Vector2 jumpDirection = transform.up * jumpForce;

            // 应用跳跃力
            rb.AddForce(jumpDirection, ForceMode2D.Impulse);

            // 触发跳跃动画（如果有）
            if (hasAnimator)
            {
                animator.SetTrigger("Jump");
            }

            Debug.Log($"跳跃！方向: {jumpDirection}, 玩家旋转: {transform.eulerAngles.z}");
        }
    }
    // 使用物理系统移动


    void MoveWithPhysics()
    {

        //Vector3 localMove = transform.right * moveInput.x * moveSpeed;             // 基于玩家的局部坐标系计算移动方向
        Vector3 localMove = new Vector3(1, 0, 0) * moveInput.x * Acceleration;                             // 不基于玩家的局部坐标系计算移动方向
        // 计算目标速度与实际速度的差异
        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 velocityDifference = new Vector2(localMove.x, 0) - new Vector2(currentVelocity.x, 0);

        // 根据速度差异计算需要施加的力
        // 使用加速度参数来控制力的强度


        //forceToApply = velocityDifference * acceleration;
        
        Vector2 forceToApply=forceToApply = new Vector2(localMove.x, 0);
        
        //float maxForce = moveSpeed * acceleration;
        //if (forceToApply.magnitude > maxForce)
        //{
        //    forceToApply = forceToApply.normalized * maxForce;
        //}

        float forceMultiplier = isGrounded ? 1f : 0.5f; // 空中控制力减半
        if (HalfForce)
        {
            forceToApply *= forceMultiplier;
        }
        rb.AddForce(forceToApply, ForceMode2D.Force);

        if (Mathf.Abs(moveInput.x) < 0.1f && isGrounded)
        {
            Vector2 frictionForce = -new Vector2(currentVelocity.x, 0) * friction;
            rb.AddForce(frictionForce, ForceMode2D.Force);
        }

        if (Mathf.Abs(rb.linearVelocity.x-PlatformVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed - PlatformVelocity.x, rb.linearVelocity.y);
            //Debug.Log("rb.linearVelocity " + rb.linearVelocity);
        }
    }

    //void MoveWithPhysics()
    //{
    //    // 基于玩家的局部坐标系计算移动方向
    //    Vector3 localMove = new Vector3(1, 0, 0) * moveInput.x * moveSpeed;

    //    // 计算目标速度与实际速度的差异
    //    Vector2 currentVelocity = rb.linearVelocity;
    //    Vector2 velocityDifference = new Vector2(localMove.x, 0) - new Vector2(currentVelocity.x, 0);

    //    // 根据速度差异计算需要施加的力
    //    Vector2 forceToApply = velocityDifference * acceleration;

    //    // 根据玩家是否在地面上调整力的强度
    //    float forceMultiplier = isGrounded ? 1f : 0.5f; // 空中控制力减半
    //    forceToApply *= forceMultiplier;

    //    // 限制最大力
    //    float maxForce = moveSpeed * acceleration;
    //    if (forceToApply.magnitude > maxForce)
    //    {
    //        forceToApply = forceToApply.normalized * maxForce;
    //    }

    //    // 对玩家施加力
    //    rb.AddForce(forceToApply, ForceMode2D.Force);

    //    // 应用摩擦力（当没有输入时且在地面上）
    //    //if (Mathf.Abs(moveInput.x) < 0.1f && isGrounded)
    //    //{
    //    //    // 施加与当前速度方向相反的摩擦力
    //    //    Vector2 frictionForce = -new Vector2(currentVelocity.x, 0) * friction;
    //    //    rb.AddForce(frictionForce, ForceMode2D.Force);
    //    //}

    //    // 限制最大水平速度
    //    if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
    //    {
    //        rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
    //    }
    //}

    void FlipCharacter()
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        if (graphics != null)
        {
            Vector3 scale = graphics.localScale;
            scale.x *= -1;
            graphics.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // 处理动画
    void HandleAnimation()
    {
        if (!hasAnimator) return;

        float speed = usePhysics ? Mathf.Abs(rb.linearVelocity.x) : Mathf.Abs(currentSpeed);
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // 公共方法
    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (rb != null && !enabled)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public float GetCurrentSpeed()
    {
        return usePhysics ? rb.linearVelocity.x : currentSpeed;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(GetCurrentSpeed()) > 0.1f;
    }

    // 新的Input System事件处理（可选）
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!InhibitInput && value.isPressed)
        {
            jumpPressed = true;
        }
    }

    void OnDestroy()
    {
        // 清理资源
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.Disable();
        }
    }

    public void FlipPlayerUpsideDown(float RotationDuration, float abnormalGravity)
    {
        rb.gravityScale = abnormalGravity;
    }

    public void RestorePlayerRotation(float RotationDuration)
    {
        rb.gravityScale = 10f;
    }

    public IEnumerator SmoothRestoreRotation(float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        RestoringRotation = true;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, originalPlayerRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = originalPlayerRotation;
        RestoringRotation = false;
    }

    private IEnumerator SmoothFlipPlayer(float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 180f);
        FlipingPlayer = true;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        FlipingPlayer = false;
    }

    void CheckRotation()
    {
        if (playerInUnomalyArea)
        {
            if (transform.rotation != Quaternion.Euler(0, 0, 180f) && !RestoringRotation)
            {
                Debug.Log(transform.rotation + "transform.rotation");
                Debug.Log("playerInUnomalyArea");
                StartCoroutine(SmoothFlipPlayer(0.1f));
            }
        }
        else
        {
            if (transform.rotation != originalPlayerRotation && !FlipingPlayer)
            {
                StartCoroutine(SmoothRestoreRotation(0.1f));
            }
        }
    }
}