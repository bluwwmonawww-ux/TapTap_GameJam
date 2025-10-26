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

    [Header("组件引用")]
    [SerializeField] private Button continueButton;

    private CanvasGroup fadeCanvasGroup;
    private const string PLAYER_DATA_FILE_NAME = "PlayerData";
    private bool isSceneLoaded = false;
    private int frameCount = 0;
    private PlayerActionFrame savedData;

    private void Start()
    {
        // 如果未在Inspector中分配按钮，自动获取
        if (continueButton == null)
            continueButton = GetComponent<Button>();

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        else
        {
            Debug.LogError("ContinueGameButton 必须挂载在Button组件上或手动分配Button引用!");
        }
    }

    // Button调用的方法（必须是void）
    public void OnContinueButtonClicked()
    {
        // 检查存档是否存在
        savedData = SaveSystemTutorial.SaveSystem.LoadFromJson<PlayerActionFrame>(PLAYER_DATA_FILE_NAME);

        if (savedData != null)
        {
            Debug.Log($"找到存档，开始加载场景: {savedData.sceneName}");
            // 启动协程加载游戏
            StartCoroutine(LoadSavedGameWithFade(savedData));
        }
        else
        {
            Debug.LogWarning("没有找到存档文件，无法继续游戏");
            // 可以在这里添加UI提示，比如显示"没有找到存档"
        }
    }

    // 协程方法 - 实际加载逻辑
    private IEnumerator LoadSavedGameWithFade(PlayerActionFrame saveData)
    {
        // 创建淡化画布
        fadeCanvasGroup = CreateFadeCanvas();

        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 淡出效果
        yield return StartCoroutine(FadeToBlack(fadeDuration));

        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.sceneName);
        asyncLoad.allowSceneActivation = true;

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("场景异步加载完成");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}");
        // 取消注册，避免重复调用
        SceneManager.sceneLoaded -= OnSceneLoaded;
        isSceneLoaded = true;
        frameCount = 0;
    }

    private void Update()
    {
        // 只有在新场景加载完成后才处理玩家位置设置
        if (!isSceneLoaded || savedData == null)
            return;

        if (frameCount == 0)
        {
            Debug.Log("frameCount = 0，准备延迟一帧");
            frameCount++;
            return;
        }

        if (frameCount == 1)
        {
            Debug.Log("frameCount = 1，开始设置玩家位置");
            frameCount = -1; // 标记已处理
            StartCoroutine(SetPlayerPositionAndFadeIn());
        }
    }

    private IEnumerator SetPlayerPositionAndFadeIn()
    {
        // 设置玩家位置
        yield return StartCoroutine(SetPlayerPositionFromSave(savedData));

        // 淡入效果
        yield return StartCoroutine(FadeInFromBlack(fadeDuration));

        // 清理
        Destroy(gameObject);
    }

    private IEnumerator SetPlayerPositionFromSave(PlayerActionFrame saveData)
    {
        GameObject player = null;
        int maxAttempts = 10;
        int attempts = 0;

        // 多次尝试查找玩家
        while (player == null && attempts < maxAttempts)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                attempts++;
                Debug.Log($"第{attempts}次尝试查找玩家...");
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (player != null)
        {
            player.transform.position = saveData.position;
            player.transform.rotation = saveData.rotation;

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
            Debug.LogError($"在{maxAttempts}次尝试后仍然找不到玩家!");
        }
    }

    // 修改返回类型为 CanvasGroup
    private CanvasGroup CreateFadeCanvas()
    {
        // 先检查是否已存在
        GameObject existingCanvas = GameObject.Find("FadeCanvas");
        if (existingCanvas != null)
        {
            Destroy(existingCanvas);
        }

        // 创建 Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建 Image
        GameObject fadeImageObj = new GameObject("FadeImage");
        fadeImageObj.transform.SetParent(canvasObj.transform, false);

        Image fadeImage = fadeImageObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        RectTransform rectTransform = fadeImageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        CanvasGroup canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        DontDestroyOnLoad(canvasObj);

        Debug.Log("淡化 Canvas 创建成功");

        return canvasGroup; // 直接返回 CanvasGroup
    }

    private IEnumerator FadeToBlack(float duration)
    {
        Debug.Log("淡出协程开始");
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
        Debug.Log("淡入协程开始");
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