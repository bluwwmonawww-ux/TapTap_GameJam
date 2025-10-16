using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[System.Serializable]
public class PlayerActionFrame
{
    public float timestamp;
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 moveDir;
    public bool isGrounded;
    public float currentSpeed;
    public bool isJumping;
    public Vector2 velocity;
    
    public PlayerActionFrame(float time, Vector3 pos, Quaternion rot, Vector2 move, bool ground, float speed, bool jump, Vector2 vel)
    {
        timestamp = time;
        position = pos;
        rotation = rot;
        moveDir = move;
        isGrounded = ground;
        currentSpeed = speed;
        isJumping = jump;
        velocity = vel;
    }
}

public class PlayerActionRecorder : MonoBehaviour
{
    [Header("记录设置")]
    [SerializeField] private float recordDuration = 2f; // 记录2秒的数据
    [SerializeField] private float recordInterval = 0.02f;
    const string PLAYER_DATA_FILE_NAME = "PlayerData";
    private Queue<PlayerActionFrame> actionHistory = new Queue<PlayerActionFrame>();
    private PlayerController playerController;
    private PhysicCheck physicsCheck;
    private PlayerCommand playerCommand;
    private Rigidbody2D playerRigidBody;
    
    private float lastRecordTime;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        physicsCheck = GetComponent<PhysicCheck>();
        playerCommand = GetComponent<PlayerCommand>();
        playerRigidBody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        RecordPlayerAction();
        CleanOldRecords();
    }
    
    private void RecordPlayerAction()
    {
        if (Time.time - lastRecordTime >= recordInterval)
        {
            Vector3 currentPos = transform.position;
            Quaternion currentRot = transform.rotation;
            Vector2 currentMoveDir = playerCommand.moveDir;
            bool currentGrounded = physicsCheck.isGround;
            float currentSpeed = playerController.CurrentSpeed;
            bool currentJumping = playerRigidBody.linearVelocity.y > 0.1f;
            Vector2 currentVelocity = playerRigidBody.linearVelocity;
            PlayerActionFrame frame = new PlayerActionFrame(
                Time.time, currentPos, currentRot, currentMoveDir, 
                currentGrounded, currentSpeed, currentJumping, currentVelocity
            );
            
            actionHistory.Enqueue(frame);
            lastRecordTime = Time.time;
        }
    }
    
    private void CleanOldRecords()
    {
        float cutoffTime = Time.time - recordDuration;
        while (actionHistory.Count > 0 && actionHistory.Peek().timestamp < cutoffTime)
        {
            actionHistory.Dequeue();
        }
    }
    
    public PlayerActionFrame GetActionFrame(float timeOffset)
    {
        float targetTime = Time.time - timeOffset;
        
        foreach (var frame in actionHistory)
        {
            if (frame.timestamp >= targetTime)
            {
                return frame;
            }
        }
        
        return null; 
    }
    
    public int GetHistoryCount()
    {
        return actionHistory.Count;
    }
    
    public void ClearHistory()
    {
        actionHistory.Clear();
    }

    public void Save()
    {
        if (actionHistory.Count > 0)
        {
            // 保存队列最后一帧
            PlayerActionFrame lastFrame = null;
            foreach (var frame in actionHistory)
            {
                lastFrame = frame;
            }
            SaveSystemTutorial.SaveSystem.SaveByJson(PLAYER_DATA_FILE_NAME, lastFrame);
            Debug.Log("已保存玩家状态");
        }
    }
    public void Load()
    {
        var saveData = SaveSystemTutorial.SaveSystem.LoadFromJson<PlayerActionFrame>(PLAYER_DATA_FILE_NAME);
        if (saveData != null)
        {

            transform.position = saveData.position;
            transform.rotation = saveData.rotation;

            Debug.Log("已读取玩家状态");
        }
    }
}

