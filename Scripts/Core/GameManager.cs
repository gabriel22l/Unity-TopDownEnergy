using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //Input -> UIController
    [SerializeField] private PlayerContext playerContext;
    [SerializeField] private UIController uiController;
    
    [SerializeField] private ApplicationController applicationController;
    
    private void Awake()
    {   //wire input to UI
        
        Application.targetFrameRate = 60;

        GameManagerVM gameManagerVm = new GameManagerVM(this);
        
        uiController.Initialize(playerContext,  gameManagerVm);
        playerContext.Initialize(uiController);
        playerContext.PlayerInput.Initialize(uiController);
    }
    private void Start()
    {
        Pause(false);
    }
    //Momentary
    private void Update()
    {
        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            ScreenCapture.CaptureScreenshot("screenshot.png");
            Debug.Log("Screenshot taken");
        }
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
    public void QuitToMainMenu()
    {
        applicationController?.LoadMainMenuScene();
    }
}
