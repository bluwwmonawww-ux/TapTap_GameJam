using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void OnQuitButtonClick()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}