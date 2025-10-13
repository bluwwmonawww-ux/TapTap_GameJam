using UnityEngine;

public class HeightTrackMono : MonoBehaviour
{
    [Header("跳跃高度追踪")]
    [SerializeField] private bool trackJumpHeight = true;

    private float jumpStartHeight;
    private float currentJumpHeight;
    private float maxJumpHeight;
    private bool isTrackingJump = false;

    // 在跳跃开始时调用
    public void StartJumpTracking()
    {
        if (trackJumpHeight)
        {
            jumpStartHeight = transform.position.y;
            currentJumpHeight = 0f;
            maxJumpHeight = 0f;
            isTrackingJump = true;
        }
    }

    // 在 FixedUpdate 中更新高度
    private void FixedUpdate()
    {
        UpdateJumpHeight();
    }
    void UpdateJumpHeight()
    {
        if (isTrackingJump && !gameObject.GetComponent<PlayerMovement>().isGrounded)
        {
            currentJumpHeight = transform.position.y - jumpStartHeight;
            if (currentJumpHeight > maxJumpHeight)
            {
                maxJumpHeight = currentJumpHeight;
                Debug.Log("本次跳跃最大高度maxJumpHeight" + maxJumpHeight);
            }
        }
    }

    // 在落地时调用
    public void EndJumpTracking()
    {
        if (isTrackingJump)
        {
            isTrackingJump = false;
            Debug.Log("本次跳跃最大高度: " + maxJumpHeight.ToString("F2"));

            // 重置追踪
            currentJumpHeight = 0f;
            maxJumpHeight = 0f;
        }
    }

    // 然后在适当的地方调用这些方法：
    // - 在 Jump() 方法中调用 StartJumpTracking()
    // - 在 FixedUpdate 中调用 UpdateJumpHeight()
    // - 在 CheckGrounded() 中检测到落地时调用 EndJumpTracking()
}
