using UnityEngine;
using UnityEngine.SceneManagement;
using SaveSystemTutorial;

public class SceneSaveUpdater : MonoBehaviour
{
    const string PLAYER_DATA_FILE_NAME = "PlayerData";

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 查找带有Save标签的对象
        GameObject saveObj = GameObject.FindGameObjectWithTag("Save");
        if (saveObj != null)
        {
            // 构造新的存档数据
            Vector3 pos = saveObj.transform.position;
            Quaternion rot = saveObj.transform.rotation;
            string sceneName = scene.name;

            // 只保存位置和场景名（如需更多数据可扩展）
            var frame = new PlayerActionFrame(
                Time.time, pos, rot, Vector2.zero, false, 0, false, Vector2.zero, sceneName
            );
            SaveSystem.SaveByJson(PLAYER_DATA_FILE_NAME, frame);
            Debug.Log($"已自动保存新场景存档，位置：{pos}，场景：{sceneName}");
        }
        else
        {
            Debug.LogWarning("未找到带有Save标签的对象，未自动保存场景信息。");
        }
    }
}
