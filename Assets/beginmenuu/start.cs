// ��Ľű� MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("��ʼ��Ϸ��ť�������");
        SceneManager.LoadScene("startscene");
    }
}