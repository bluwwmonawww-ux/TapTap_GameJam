using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("重力参数")]
    [SerializeField] private float gravity;
    [SerializeField] private float baseGravityScale;
    [SerializeField] private float fallGravityScale = 3f;    // 下落重力倍数
    
    [Header("移动参数")]
    [SerializeField] private float maxRunSpeed = 10f;      // 奔跑速度
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float maxJumpBufferTime = 0.2f;
    [SerializeField] private float moveToMaxTime = 0.2f;
    [SerializeField] private float stopToZeroTime = 0.1f;
    [SerializeField] private float coyoteTime;

    [Header("组件引用")] 
    [SerializeField] private PhysicCheck physicsCheck;
    [SerializeField] private PlayerCommand command;
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private Animator playerAnimator;
    
    private float _currentSpeed;

    private float _jumpBufferCounter;

    private float _accelerationRate;
    
    private float _decelerationRate;
    
    private float _coyoteTimeCounter;
    void Start()
    {
        physicsCheck = gameObject.GetComponent<PhysicCheck>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        command = GetComponent<PlayerCommand>();
        
        gravity = Physics2D.gravity.y;
        baseGravityScale = playerRigidBody.gravityScale;
        _accelerationRate = maxRunSpeed / moveToMaxTime;
        _decelerationRate = maxRunSpeed / stopToZeroTime;
    }

    private void Update()
    {
        JumpBufferCalculate();
        MoveSpeedCalculate();
        CoyoteTimeCalculate();
        UpdateGravity();
    }

    void FixedUpdate()
    {
        PlayerMovement();
        if (CanJump())
        {
            PlayerJump();
        }
    }
    
    private void PlayerMovement()
    {
        playerRigidBody.linearVelocityX = command.moveDir.x * _currentSpeed;
    }
    
    private void PlayerJump()
    {
        float yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        playerRigidBody.linearVelocityY = yVelocity;
    }
    
    /// <summary>
    /// 跳跃缓冲计算
    /// </summary>
    private void JumpBufferCalculate()
    {
        if (command.isJumpPressed)
        {
            _jumpBufferCounter = maxJumpBufferTime;
        }
        
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 平滑加速和减速
    /// </summary>
    private void MoveSpeedCalculate()
    {
        // 确定目标速度
        if (Mathf.Abs(command.moveDir.x) > 0.1f)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxRunSpeed, _accelerationRate * Time.deltaTime);
        } else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _decelerationRate * Time.deltaTime);
        }
    }
    
    
    /// <summary>
    /// 土狼时间计算
    /// </summary>
    private void CoyoteTimeCalculate()
    {
        if (physicsCheck.isGround)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        if (_coyoteTimeCounter > 0)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 判断是否可以跳跃
    /// </summary>
    private bool CanJump()
    {
        return _jumpBufferCounter > 0 && (_coyoteTimeCounter > 0 || physicsCheck.isGround);
    }
    
    /// <summary>
    /// 下落时修改重力缩放
    /// </summary>
    private void UpdateGravity()
    {
        // 在地面时重置重力缩放
        if (physicsCheck.isGround && playerRigidBody.linearVelocity.y <= 0)
        {
            playerRigidBody.gravityScale = baseGravityScale;
            return;
        }
        // 下落时更强的重力
        playerRigidBody.gravityScale = playerRigidBody.linearVelocity.y < 0 ? fallGravityScale : baseGravityScale;
    }
}