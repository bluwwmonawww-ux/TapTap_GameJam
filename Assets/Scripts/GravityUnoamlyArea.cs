using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoamlyGravityArea : MonoBehaviour
{
    [Header("重力异常设置")]
    [SerializeField] private float abnormalGravity = -2f; // 异常重力值

    private float originalGravity; // 存储原始重力值
    private PlayerMovement playerMovement; // 玩家移动组件引用
    [SerializeField] private Transform playerTransform; // 玩家变换组件
    [SerializeField] private Quaternion originalPlayerRotation; // 存储玩家原始旋转

    // 当其他碰撞体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.CompareTag("Player"))
        {
            // 获取PlayerMovement组件
            playerMovement = other.GetComponent<PlayerMovement>();
            playerTransform = other.transform;

            if (playerMovement != null)
            {
                playerMovement.playerInUnomalyArea = true;

                originalPlayerRotation = Quaternion.Euler(0, 0, 0);              //playerTransform.rotation;              //确定(0, 0, 0)为初始旋转

                
                other.GetComponent<PlayerMovement>().FlipPlayerUpsideDown(0.1f, abnormalGravity);

                    
                
            }
        }
    }

    // 当其他碰撞体离开触发器时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的对象是否是玩家
        if (other.CompareTag("Player") )
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement.playerInUnomalyArea)
            {



                other.GetComponent<PlayerMovement>().RestorePlayerRotation(0.1f);




                playerMovement.playerInUnomalyArea = false;
                playerMovement = null;
            }

        }
    }

    private void OnDrawGizmos()
    {
        // 设置重力异常区域的可视化颜色
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // 半透明橙色
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
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
                Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = (CircleCollider2D)collider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(circleCollider.offset, circleCollider.radius);

                // 绘制边框
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
                Gizmos.DrawWireSphere(circleCollider.offset, circleCollider.radius);
            }
        }

        // 绘制重力方向指示器
        DrawGravityDirectionIndicator();
    }

    // 绘制重力方向指示器
    private void DrawGravityDirectionIndicator()
    {
        Vector3 areaCenter = transform.position;
        Vector3 gravityDirection = (abnormalGravity >= 0) ? Vector3.down : Vector3.up;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(areaCenter, gravityDirection * 2f);

        // 绘制箭头
        Vector3 arrowTip = areaCenter + gravityDirection * 2f;
        Vector3 perpendicular = new Vector3(-gravityDirection.y, gravityDirection.x, 0) * 0.3f;
        Gizmos.DrawLine(arrowTip, arrowTip - gravityDirection * 0.5f + perpendicular);
        Gizmos.DrawLine(arrowTip, arrowTip - gravityDirection * 0.5f - perpendicular);
    }
}