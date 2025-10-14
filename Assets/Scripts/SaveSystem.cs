using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    }
}