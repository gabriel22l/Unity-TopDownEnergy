using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    private string MainMenuScene = "MainMenu";
    private string GameScene = "Game";

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene(GameScene);
    }
    public void QuitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
