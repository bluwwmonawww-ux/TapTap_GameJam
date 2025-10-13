using UnityEngine;

public class PhysicCheck : MonoBehaviour
{
    [Header("检测参数")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset;

    [Header("玩家状态")]
    public bool isGround;

    private void Update()
    {
        isGround = CheckGround();
    }
    
    private bool CheckGround()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + groundCheckOffset, groundCheckRadius, groundMask);
    }
    
    /// <summary>
    /// 可视化地面检测
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGround ? Color.green : Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundCheckOffset ,groundCheckRadius);
    }
}