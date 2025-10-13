using UnityEngine;

public class PlayerCommand : MonoBehaviour
{
    [Header("玩家输入系统")]
    PlayerInputSystem playerInput;
    
    public Vector2 moveDir => playerInput.Player.Move.ReadValue<Vector2>();

    public bool isJumpPressed => playerInput.Player.Jump.WasPressedThisFrame();

    private void Awake()
    {
        playerInput = new PlayerInputSystem();
    }
    

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}