using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void OnQuitButtonClick()
    {
        Debug.Log("ÍË³öÓÎÏ·");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}