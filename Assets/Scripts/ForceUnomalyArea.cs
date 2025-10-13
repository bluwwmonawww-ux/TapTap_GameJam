using UnityEngine;

public class UnomalyForceArea : MonoBehaviour
{
    [Header("力场设置")]
    [SerializeField] private Vector2 force ; // 要施加的力
    [SerializeField] private bool restoreOnExit = true; // 离开时是否恢复原力

    private Vector2 originalForce; // 存储原始力
    private PlayerMovement playerMovement; // 玩家移动组件引用
    private bool playerInArea = false; // 玩家是否在区域内

    // 当其他碰撞体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.CompareTag("Player"))
        {
            // 获取PlayerMovement组件
            playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerInArea = true;

                // 保存原始的力值
                //originalForce = playerMovement.Force;

                // 应用新的力
                playerMovement.Force += force;

                Debug.Log($"进入力场区域，力从 {originalForce} 变为 {force}");
            }
        }
    }

    // 当其他碰撞体离开触发器时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的对象是否是玩家
        if (other.CompareTag("Player") && playerInArea)
        {
            if (restoreOnExit && playerMovement != null)
            {
                // 恢复原始力
                playerMovement.Force -= force;
                Debug.Log($"离开力场区域，力恢复为 {originalForce}");
            }

            playerInArea = false;
            playerMovement = null;
        }
    }

    // 在Inspector中可视化触发器区域
    private void OnDrawGizmos()
    {
        // 设置力场区域的可视化颜色
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.3f); // 半透明蓝色
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            // 根据碰撞体类型绘制不同的形状
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = (BoxCollider2D)collider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.offset, boxCollider.size);

                // 绘制边框
                Gizmos.color = new Color(0f, 0.8f, 1f, 0.8f);
                Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = (CircleCollider2D)collider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(circleCollider.offset, circleCollider.radius);

                // 绘制边框
                Gizmos.color = new Color(0f, 0.8f, 1f, 0.8f);
                Gizmos.DrawWireSphere(circleCollider.offset, circleCollider.radius);
            }
        }

        // 绘制力方向指示器
        DrawForceDirectionIndicator();
    }

    // 绘制力方向指示器
    private void DrawForceDirectionIndicator()
    {
        Vector3 areaCenter = transform.position;

        // 标准化力向量以显示方向
        Vector3 forceDirection = force.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(areaCenter, forceDirection * 2f);

        // 绘制箭头
        Vector3 arrowTip = areaCenter + forceDirection * 2f;
        Vector3 perpendicular = new Vector3(-forceDirection.y, forceDirection.x, 0) * 0.3f;
        Gizmos.DrawLine(arrowTip, arrowTip - forceDirection * 0.5f + perpendicular);
        Gizmos.DrawLine(arrowTip, arrowTip - forceDirection * 0.5f - perpendicular);

        // 显示力的大小
#if UNITY_EDITOR
        UnityEditor.Handles.Label(areaCenter + forceDirection * 1.2f, $"Force: {force.magnitude:F1}");
#endif
    }
}