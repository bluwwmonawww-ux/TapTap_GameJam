using System;
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
    
    [Header("爬墙参数")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer = 1; // 墙壁层
    [SerializeField] private float wallSlideSpeed = 2f; // 墙壁下滑速度
    [SerializeField] private float wallJumpForce = 10f; // 墙壁跳跃力度
    [SerializeField] private float wallStickForce = 5f; // 墙壁吸附力度
    [SerializeField] private float maxWallClimbSpeed = 3f; // 爬墙时最大垂直速度
    [Header("冲刺参数")]
    [SerializeField]private float RushDuration=0.15f;
    [SerializeField] private float RushDistance = 5;
    [SerializeField] private float RushCD;
    [Header("空中控制力是否减半")]
    [SerializeField] private bool HalfForce;
    [Header("物理参数")]
    //[SerializeField] private float friction = 0.2f;
    [SerializeField] private bool usePhysics = true;
    [SerializeField] public float Gravity = 98f;
    [SerializeField] public float FrictionPRESET=1f;
    [SerializeField] public float Friction=1f;
    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform graphics;

    
    [Header("输入设置")]
    [SerializeField] private InputAction playerInput;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference ShiftAction;

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
    [SerializeField] private bool jumpPressed;
     [SerializeField]private bool jumpPressedLastFrame; // 跟踪上一帧的跳跃输入状态
    [SerializeField] public  bool InverseAD;

    // AD键失控相关变量
    [SerializeField] private bool isADOutOfControl = false;  // 是否AD键失控
    [SerializeField] private bool forceLeftInput = false;    // 是否强制向左输入
    [SerializeField] private bool forceRightInput = false;   // 是否强制向右输入
    
    // 爬墙相关变量***********************
    public bool isWallSliding;
    public bool isWallClimbing;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool wallJumping;
    private float wallJumpTimer;

   // 爬墙相关变量*****************************
    private Vector2 PlatformVelocity;
    private Vector2 LastFPlatformVelocity;
    [SerializeField] private int RushTime;
    private bool Rush;
    private bool Unforced;
    void Start()
    {
        RushTime = 1;
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
        
        
        HandleAnimation();
        FlipCharacter();
       
    }

    void FixedUpdate()
    {
        GetInput();
        PlatformVelocity = Vector2.zero;
        CheckGrounded();
        CheckWalls();

        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
        }
        if(jumpPressed){

            Debug.Log("isgrounded"+isGrounded);
        }
        if (jumpPressed && (isGrounded || isWallSliding))
        {
            Debug.Log("jumpPressedjumpPressedjumpPressedjumpPressed");
            Jump();
            jumpPressed = false;
        }
        
        if (Rush && RushCD<=0) {
            Debug.Log("Rush"+ moveInput);
            StartCoroutine(RUSH(moveInput));
            Rush = false;
            RushTime = 0;
            RushCD = 0.2f;
        }else if (RushCD > 0)
        {
            RushCD-=Time.deltaTime;
        }

        
        LastFPlatformVelocity = PlatformVelocity;
        if ( !Unforced)
        {
            rb.AddForce(Force);
            rb.AddForce(new Vector2(0,- Gravity));
        }
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
        if (ShiftAction != null)
        {
            ShiftAction.action.Enable();
        }
    }

    // 获取输入
    void GetInput()
    {
        
        if (!InhibitInput)
        {
            // 检查是否AD键失控
            if (isADOutOfControl)
            {
                // AD键失控时，使用强制输入
                if (forceLeftInput)
                {
                    moveInput = new Vector2(-1f, 0f); // 强制向左
                }
                else if (forceRightInput)
                {
                    moveInput = new Vector2(1f, 0f);  // 强制向右
                }
                else
                {
                    moveInput = Vector2.zero; // 无输入
                }
            }
            else
            {
                // 正常输入
                moveInput = moveAction.action.ReadValue<Vector2>();
            }
            
            if (InverseAD)
            {
                moveInput = new Vector2(-moveInput.x, moveInput.y);
            }

            float jumpValue = jumpAction.action.ReadValue<float>();
            
            // 真正的按下触发逻辑（类似GetKeyDown）
            bool jumpInputThisFrame = jumpValue > 0.5f;
            if (jumpInputThisFrame){
            Debug.Log("jumpInputThisFrame"+jumpInputThisFrame);
            
            Debug.Log("jumpPressedLastFrame"+jumpPressedLastFrame);
            }

            if (jumpInputThisFrame && !jumpPressedLastFrame)
            {
                jumpPressed = true; // 只在按下瞬间触发
            }
            else
            {
                jumpPressed = false;
            }
            
            jumpPressedLastFrame = jumpInputThisFrame; // 记录这一帧的状态
            float shiftvalue =ShiftAction.action.ReadValue<float>();
            
            if (shiftvalue>0 && RushTime>0)
            {
                Debug.Log(shiftvalue);
                Rush = true;
                
            }
            else
            {
                    Rush = false;
            }
        }
        else
        {
                Rush = false;
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
                    if (RushTime == 0) {
                        RushTime = 1;
                    }
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

    // 检查墙壁接触
    void CheckWalls()
    {
        Vector2 rayStart = (Vector2)transform.position;
        
        // 获取角色碰撞体的边界
        Collider2D playerCollider = GetComponent<Collider2D>();
        float playerWidth = playerCollider != null ? playerCollider.bounds.extents.x : 0.5f;
        
        // 从角色左右两边稍微延伸一点开始检测
        Vector2 leftRayStart = rayStart - (Vector2)transform.right * (playerWidth + 0.1f);
        Vector2 rightRayStart = rayStart + (Vector2)transform.right * (playerWidth + 0.1f);
        
        // 使用 RaycastAll 检测左侧所有碰撞体
        RaycastHit2D[] leftWallHits = Physics2D.RaycastAll(leftRayStart, -(Vector2)transform.right, wallCheckDistance, wallLayer);
        bool foundLeftWall = false;
        
        // 遍历所有命中的左侧碰撞体
        foreach (RaycastHit2D hit in leftWallHits)
        {
            if (hit.collider != null && hit.collider.GetComponent<ClimableWall>() != null)
            {
                foundLeftWall = true;
                break;
            }
        }
        
        // 使用 RaycastAll 检测右侧所有碰撞体
        RaycastHit2D[] rightWallHits = Physics2D.RaycastAll(rightRayStart, (Vector2)transform.right, wallCheckDistance, wallLayer);
        bool foundRightWall = false;
        
        // 遍历所有命中的右侧碰撞体
        foreach (RaycastHit2D hit in rightWallHits)
        {
            if (hit.collider != null && hit.collider.GetComponent<ClimableWall>() != null)
            {
                foundRightWall = true;
                break;
            }
        }
        
        isTouchingLeftWall = foundLeftWall;
        isTouchingRightWall = foundRightWall;
        
        // 判断是否在爬墙（需要按住A或D键）
        bool isHoldingWallInput = (isTouchingLeftWall && moveInput.x < -0.1f) || (isTouchingRightWall && moveInput.x > 0.1f);
        isWallSliding = (isTouchingLeftWall || isTouchingRightWall) && !isGrounded && isHoldingWallInput;
        isWallClimbing = isWallSliding && Mathf.Abs(moveInput.x) > 0.1f;
        
        // 爬墙时限制垂直方向最大速度
        if (isWallSliding)
        {
            if (Mathf.Abs(rb.linearVelocity.y) > maxWallClimbSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Sign(rb.linearVelocity.y) * maxWallClimbSpeed);
            }
        }
        
        // 墙壁跳跃计时器
        if (wallJumping)
        {
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer > 0.2f)
            {
                wallJumping = false;
                wallJumpTimer = 0f;
            }
        }
        
        // 调试可视化
        Debug.DrawRay(leftRayStart, -(Vector2)transform.right * wallCheckDistance, isTouchingLeftWall ? Color.green : Color.red, 0.1f);
        Debug.DrawRay(rightRayStart, (Vector2)transform.right * wallCheckDistance, isTouchingRightWall ? Color.green : Color.red, 0.1f);
    }

    // 跳跃方法
    
    void Jump()
    {
        if (rb != null && isGrounded)
        {
            // 总是使用玩家的向上方向作为跳跃方向
            //gameObject.GetComponent<HeightTrackMono>().StartJumpTracking();
            Vector2 jumpDirection = transform.up * jumpForce;

            rb.AddForce(jumpDirection, ForceMode2D.Impulse);

            // 触发跳跃动画（如果有）
            if (hasAnimator)
            {
                animator.SetTrigger("Jump");
            }

            Debug.Log($"跳跃！方向: {jumpDirection}, 玩家旋转: {transform.eulerAngles.z}");
        }
    }


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

        // 爬墙时的移动限制
        if (isWallSliding)
        {
            // 爬墙时限制向墙壁方向的移动
            if (isTouchingLeftWall && moveInput.x < -0.1f)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                rb.AddForce(new Vector2(0,0.8f*Gravity),ForceMode2D.Force);
            }
            else if (isTouchingRightWall && moveInput.x > 0.1f)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                rb.AddForce(new Vector2(0,0.8f*Gravity),ForceMode2D.Force);
            }
        }

        if (Mathf.Abs(moveInput.x) < 0.1f && !jumpPressed && isGrounded && !Unforced)
        {
            
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, PlatformVelocity, Friction);
        }

        if (Mathf.Abs(rb.linearVelocity.x-PlatformVelocity.x) > maxSpeed && !Unforced)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed - PlatformVelocity.x, rb.linearVelocity.y);
            //Debug.Log("rb.linearVelocity " + rb.linearVelocity);
        }
        //if (rb.linearVelocity.y > 0 && Mathf.Abs(rb.linearVelocity.y - PlatformVelocity.y) > maxSpeed * 2f && !Unforced)
        //{
        //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Sign(rb.linearVelocity.y) * maxSpeed * 2f - PlatformVelocity.y);
        //    Debug.Log("rb.linearVelocity " + rb.linearVelocity);
        //}
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
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("IsWallClimbing", isWallClimbing);
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
        Gravity = abnormalGravity;
    }

    public void RestorePlayerRotation(float RotationDuration)
    {
        Gravity = 98f;
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
    public IEnumerator RUSH(Vector2 MoveInput)
    {
        Vector2 Dirction=moveInput.normalized;
        float time = 0;
        while (time< RushDuration)
        {
            rb.linearVelocity = Dirction * RushDistance / RushDuration;//Dirction * RushDistance / 0.2f;
            InhibitInput =true;
            Unforced=true;
            //transform.position +=(Vector3) Dirction * RushDistance / 0.2f*Time.deltaTime;
            //rb.AddForce(Dirction * RushDistance*100f);
            time += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity *= 0.2f;
        Unforced=false;
        InhibitInput=false;
    }
    
    // AD键失控控制接口
    /// <summary>
    /// 设置AD键失控状态
    /// </summary>
    /// <param name="forceLeft">是否强制向左</param>
    /// <param name="forceRight">是否强制向右</param>
    public void SetADOutOfControl(bool forceLeft, bool forceRight)
    {
        isADOutOfControl = forceLeft || forceRight;
        forceLeftInput = forceLeft;
        forceRightInput = forceRight;
        
        if (isADOutOfControl)
        {
            Debug.Log($"AD键失控设置: 强制向左={forceLeft}, 强制向右={forceRight}");
        }
        else
        {
            Debug.Log("AD键恢复正常控制");
        }
    }
    
    /// <summary>
    /// 检查AD键是否失控
    /// </summary>
    /// <returns>是否AD键失控</returns>
    public bool IsADOutOfControl()
    {
        return isADOutOfControl;
    }
    
    /// <summary>
    /// 获取当前强制输入方向
    /// </summary>
    /// <returns>强制输入方向：-1向左，1向右，0无强制</returns>
    public int GetForceInputDirection()
    {
        if (forceLeftInput) return -1;
        if (forceRightInput) return 1;
        return 0;
    }
}