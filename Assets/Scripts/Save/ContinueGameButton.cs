using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 继续游戏按钮脚本
/// 从最近的存档文件中加载场景和玩家位置
/// </summary>
public class ContinueGameButton : MonoBehaviour
{
    [Header("淡入淡出设置")]
    [SerializeField] private float fadeDuration = 1f; // 淡化时长
    
    private Button continueButton;
    private CanvasGroup fadeCanvasGroup;
    private const string PLAYER_DATA_FILE_NAME = "PlayerData";

    private void Start()
    {
        continueButton = GetComponent<Button>();
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueGameClicked);
        }
        else
        {
            Debug.LogError("ContinueGameButton 脚本必须挂载在 Button 组件上!");
        }
    }

    private void OnContinueGameClicked()
    {
        // 检查是否存在存档
        var saveData = SaveSystemTutorial.SaveSystem.LoadFromJson<PlayerActionFrame>(PLAYER_DATA_FILE_NAME);
        
        if (saveData != null)
        {
            Debug.Log($"找到存档: 场景={saveData.sceneName}, 位置={saveData.position}");
            StartCoroutine(LoadSavedGameWithFade(saveData));
        }
        else
        {
            Debug.LogWarning("没有找到存档文件，无法继续游戏");
        }
    }

    private IEnumerator LoadSavedGameWithFade(PlayerActionFrame saveData)
    {
        // 创建淡化画布
        fadeCanvasGroup = CreateFadeCanvas();
        
        // 淡出（黑屏）
        yield return StartCoroutine(FadeToBlack(fadeDuration));
        
        // 加载保存的场景
        SceneManager.LoadScene(saveData.sceneName);
        
        // 等待场景加载完成
        yield return new WaitForSeconds(0.5f);
        
        // 找到玩家并设置位置
        SetPlayerPositionFromSave(saveData);
        
        // 淡入（恢复画面）
        yield return StartCoroutine(FadeInFromBlack(fadeDuration));
    }

    private void SetPlayerPositionFromSave(PlayerActionFrame saveData)
    {
        // 查找场景中的玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // 设置位置和旋转
            player.transform.position = saveData.position;
            player.transform.rotation = saveData.rotation;
            
            // 重置 Rigidbody2D 速度
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            
            // 重新启用玩家控制
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.InhibitInput = false;
            }
            
            Debug.Log($"玩家已传送至: {saveData.position}");
        }
        else
        {
            Debug.LogWarning("场景中找不到玩家!");
        }
    }

    private CanvasGroup CreateFadeCanvas()
    {
        // 创建 Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 确保在最上层
        
        // 添加 GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 创建淡化背景 Image
        GameObject fadeImageObj = new GameObject("FadeImage");
        fadeImageObj.transform.SetParent(canvasObj.transform, false);
        
        Image fadeImage = fadeImageObj.AddComponent<Image>();
        fadeImage.color = Color.black; // 设置为黑色
        
        RectTransform rectTransform = fadeImageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        CanvasGroup canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // 初始透明
        DontDestroyOnLoad(canvasObj);
        
        Debug.Log("淡化 Canvas 创建成功");
        
        return canvasGroup;
    }

    private IEnumerator FadeToBlack(float duration)
    {
        Debug.Log("淡出开始");
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 1f; // 确保完全黑屏
        Debug.Log("淡出完成");
    }

    private IEnumerator FadeInFromBlack(float duration)
    {
        Debug.Log("淡入开始");
        float elapsed = 0f;
        
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("fadeCanvasGroup 为 null");
            yield break;
        }
        
        // 确保开始时是完全黑屏
        fadeCanvasGroup.alpha = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / duration));
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 0f; // 完全透明
        Debug.Log("淡入完成");
        
        // 销毁淡化画布
        if (fadeCanvasGroup != null)
        {
            Destroy(fadeCanvasGroup.gameObject);
        }
    }

    /// <summary>
    /// 检查是否存在存档文件
    /// </summary>
    public bool HasSaveFile()
    {
        var saveData = SaveSystemTutorial.SaveSystem.LoadFromJson<PlayerActionFrame>(PLAYER_DATA_FILE_NAME);
        return saveData != null;
    }
}
