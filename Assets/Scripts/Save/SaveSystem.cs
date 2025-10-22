using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;
}

namespace SaveSystemTutorial
{
    public static class SaveSystem
    {
        public static void SaveByJson(string saveFileName, object data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            File.WriteAllText(path, json);
            Debug.Log($"Saved to {path}");
        }

        public static T LoadFromJson<T>(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<T>(json);
                Debug.Log($"Loaded from {path}");
                return data;
            }
            Debug.LogWarning($"No save file found at {path}");
            return default;
        }

        public static void DeleteSaveFile(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            
        }
        public static void SavePlayerPosition(string saveFileName, Vector3 position)
        {
            var data = new PlayerSaveData { position = position };
            SaveByJson(saveFileName, data);
        }

        public static Vector3 LoadPlayerPosition(string saveFileName)
        {
            var data = LoadFromJson<PlayerSaveData>(saveFileName);
            return data != null ? data.position : Vector3.zero;
        }

    }
}