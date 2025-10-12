using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;          // 移动速度
    [SerializeField] private float acceleration = 10f;      // 加速度
    [SerializeField] private float deceleration = 8f;       // 减速度
    [SerializeField] private float maxSpeed = 8f;           // 最大速度

    [Header("物理参数")]
    [SerializeField] private float friction = 0.2f;         // 摩擦力
    [SerializeField] private bool usePhysics = true;        // 是否使用物理系统

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;               // 2D刚体引用
    [SerializeField] private Transform graphics;           // 视觉部分（用于翻转）

    // 内部变量
    private float horizontalInput;
    private float currentSpeed;
    private bool isFacingRight = true;

    // 动画相关（可选）
    private Animator animator;
    private bool hasAnimator;

    void Start()
    {
        // 自动获取组件（如果没有在Inspector中赋值）
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (graphics == null)
            graphics = transform.Find("Graphics"); // 假设视觉部分在名为"Graphics"的子对象中

        // 获取动画组件（可选）
        animator = GetComponent<Animator>();
        hasAnimator = animator != null;

        // 初始化速度
        currentSpeed = 0f;
    }

    void Update()
    {
        GetInput();
        HandleAnimation();
        FlipCharacter();
    }

    void FixedUpdate()
    {
        if (usePhysics && rb != null)
        {
            MoveWithPhysics();
        }
        else
        {
            MoveWithTransform();
        }
    }

    // 获取玩家输入
    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // 使用Raw获得瞬时输入

        // 调试信息（在开发时有用）
        Debug.Log($"Input: {horizontalInput}, Current Speed: {currentSpeed}");
    }

    // 使用物理系统移动（更真实）
    void MoveWithPhysics()
    {
        Vector2 targetVelocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 平滑过渡到目标速度
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // 应用摩擦力（当没有输入时）
        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x * (1 - friction), rb.velocity.y);
        }

        // 限制最大速度
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    // 使用Transform移动（更简单直接）
    void MoveWithTransform()
    {
        // 计算目标速度
        float targetSpeed = horizontalInput * moveSpeed;

        // 加速和减速
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // 加速
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // 减速
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 限制最大速度
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // 应用移动
        Vector3 movement = new Vector3(currentSpeed, 0f, 0f) * Time.fixedDeltaTime;
        transform.Translate(movement, Space.World);
    }

    // 翻转角色朝向
    void FlipCharacter()
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // 如果有单独的视觉部分，翻转它
        if (graphics != null)
        {
            Vector3 scale = graphics.localScale;
            scale.x *= -1;
            graphics.localScale = scale;
        }
        else
        {
            // 如果没有，翻转整个物体
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // 处理动画（可选）
    void HandleAnimation()
    {
        if (!hasAnimator) return;

        // 设置移动速度参数
        float speed = usePhysics ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(currentSpeed);
        animator.SetFloat("Speed", speed);

        // 设置是否在地面参数（如果需要）
        // animator.SetBool("IsGrounded", IsGrounded());
    }

    // 公共方法，用于外部控制
    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (rb != null && !enabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public float GetCurrentSpeed()
    {
        return usePhysics ? rb.velocity.x : currentSpeed;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(GetCurrentSpeed()) > 0.1f;
    }
}