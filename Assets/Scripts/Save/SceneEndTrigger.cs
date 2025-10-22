using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneEndTrigger : MonoBehaviour
{
    [Header("Ŀ�곡������")]
    public string targetSceneName = "NextScene";
    
    [Header("淡入淡出设置")]
    public float fadeDuration = 1f; // 淡化时长
    
    [Header("Bug隐藏设置")]
    public float bugHideDuration = 1f; // Bug 隐藏时长
    
    private GameObject cachedPlayer;
    private int frameCount = 0;
    private CanvasGroup fadeCanvasGroup;
    private bool isSceneLoaded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 检查场景中是否还存在标签为 "Item" 的对象
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            if (items.Length > 0)
            {
                Debug.Log($"场景中还有Item 对象，无法跳转场景");
                return;
            }
            
            cachedPlayer = collision.gameObject;
            DontDestroyOnLoad(cachedPlayer);
            DontDestroyOnLoad(gameObject); // 让 SceneEndTrigger 也保持存活
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // 开始淡出效果
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        // 禁用玩家控制
        DisablePlayerControl();
        
        // 隐藏 Bug 标签的对象
        yield return StartCoroutine(HideBugObjects(bugHideDuration));
        
        // 创建或获取淡化画布
        fadeCanvasGroup = CreateFadeCanvas();
        
        // 淡出（黑屏）
        yield return StartCoroutine(FadeToBlack(fadeDuration));
        
        // 加载新场景
        SceneManager.LoadScene(targetSceneName);
    }

    private void DisablePlayerControl()
    {
        if (cachedPlayer != null)
        {
            var playerMovement = cachedPlayer.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.InhibitInput = true;
                Debug.Log("玩家控制已禁用");
            }
        }
    }

    private IEnumerator HideBugObjects(float duration)
    {
        Debug.Log("开始隐藏 Bug 对象");
        
        // 获取所有 Bug 标签的对象
        GameObject[] bugObjects = GameObject.FindGameObjectsWithTag("Bug");
        
        if (bugObjects.Length == 0)
        {
            Debug.LogWarning("场景中没有找到 Bug 标签的对象");
            yield break;
        }
        
        Debug.Log($"找到 {bugObjects.Length} 个 Bug 对象，开始隐藏");
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / duration)); // 从 1 逐渐减到 0
            
            foreach (var bugObject in bugObjects)
            {
                if (bugObject == null) continue;
                
                // 设置 SpriteRenderer 的透明度
                var spriteRenderer = bugObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = alpha;
                    spriteRenderer.color = color;
                }
                
                // 如果有子物体，也设置其透明度
                foreach (Transform child in bugObject.transform)
                {
                    var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                    if (childSpriteRenderer != null)
                    {
                        Color color = childSpriteRenderer.color;
                        color.a = alpha;
                        childSpriteRenderer.color = color;
                    }
                }
            }
            
            Debug.Log($"Bug 隐藏中... alpha: {alpha}");
            yield return null;
        }
        
        // 确保完全隐藏
        foreach (var bugObject in bugObjects)
        {
            if (bugObject == null) continue;
            
            var spriteRenderer = bugObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0f;
                spriteRenderer.color = color;
            }
            
            foreach (Transform child in bugObject.transform)
            {
                var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null)
                {
                    Color color = childSpriteRenderer.color;
                    color.a = 0f;
                    childSpriteRenderer.color = color;
                }
            }
        }
        
        Debug.Log("Bug 对象已完全隐藏");
    }

    private CanvasGroup CreateFadeCanvas()
    {
        // 创建 Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 确保在最上层（增加到 9999）
        
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
        canvasGroup.alpha = 1f; // 初始完全不透明
        DontDestroyOnLoad(canvasObj);
        
        Debug.Log($"淡化 Canvas 创建成功，sortingOrder: {canvas.sortingOrder}, Image color: {fadeImage.color}");
        
        return canvasGroup;
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
            Debug.Log($"淡出中... elapsed: {elapsed}, alpha: {alpha}");
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 1f; // 确保完全黑屏
        Debug.Log("淡出完成，画布已全黑");
    }

    private IEnumerator FadeInFromBlack(float duration)
    {
        Debug.Log("淡入协程开始");
        float elapsed = 0f;
        
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("fadeCanvasGroup 为 null，无法继续淡入");
            yield break;
        }
        
        // 先确保是完全黑屏
        fadeCanvasGroup.alpha = 1f;
        Debug.Log($"设置为完全黑屏，alpha: {fadeCanvasGroup.alpha}");
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / duration));
            fadeCanvasGroup.alpha = alpha;
            Debug.Log($"淡入中... elapsed: {elapsed}, alpha: {alpha}");
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 0f; // 完全透明
        Debug.Log("淡入完成，画布已透明");
        
        // 销毁淡化画布
        if (fadeCanvasGroup != null)
        {
            Destroy(fadeCanvasGroup.gameObject);
            Debug.Log("淡化 Canvas 已销毁");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}, cachedPlayer: {cachedPlayer}");
        // 取消注册，避免重复调用
        SceneManager.sceneLoaded -= OnSceneLoaded;
        isSceneLoaded = true;
        frameCount = 0;
    }

    private void Update()
    {
        // 只有在新场景加载完成后才处理
        if (!isSceneLoaded)
            return;
            
        if (frameCount == 0 && cachedPlayer != null)
        {
            Debug.Log("frameCount = 0，准备延迟一帧");
            frameCount++;
            return;
        }
        
        if (frameCount == 1 && cachedPlayer != null)
        {
            Debug.Log("frameCount = 1，开始设置玩家位置");
            frameCount = -1; // 标记已处理
            SetPlayerPosition();
        }
    }

    private void SetPlayerPosition()
    {
        GameObject startPoint = GameObject.FindGameObjectWithTag("Start");
        if (startPoint != null && cachedPlayer != null)
        {
            cachedPlayer.transform.position = startPoint.transform.position;
            cachedPlayer.transform.rotation = startPoint.transform.rotation;
            Debug.Log($"玩家已传送至 Start 点: {startPoint.transform.position}");
        }
        else
        {
            Debug.LogWarning($"无法找到 Start 点或玩家对象! startPoint: {startPoint}, cachedPlayer: {cachedPlayer}");
        }
        
        // 重新启用玩家控制
        EnablePlayerControl();
        
        // 开始淡入效果
        Debug.Log($"fadeCanvasGroup: {fadeCanvasGroup}, fadeCanvasGroup is null: {fadeCanvasGroup == null}");
        if (fadeCanvasGroup != null)
        {
            Debug.Log("开始淡入效果");
            StartCoroutine(FadeInAndCleanup());
        }
        else
        {
            Debug.LogWarning("fadeCanvasGroup 为 null，无法执行淡入效果");
            Destroy(gameObject);
        }
    }

    private void EnablePlayerControl()
    {
        if (cachedPlayer != null)
        {
            var playerMovement = cachedPlayer.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.InhibitInput = false;
                Debug.Log("玩家控制已恢复");
            }
        }
    }

    private IEnumerator FadeInAndCleanup()
    {
        yield return StartCoroutine(FadeInFromBlack(fadeDuration));
        
        // 淡入完成后才销毁
        Destroy(gameObject);
    }
}
