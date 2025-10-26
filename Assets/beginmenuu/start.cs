// 你的脚本 MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("开始游戏按钮被点击！");
        SceneManager.LoadScene("startscene");
    }
}