using System;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    private GameManagerVM gameManagerVm;
    private UIController uiController;
    public void Initialize(GameManagerVM vm, UIController uiController)
    {
        this.gameManagerVm = vm;
        this.uiController = uiController;
        if (this.gameObject.activeSelf)
        {
            gameManagerVm.RequestPause();
        } // else wait for OnEnable
    }
    private void OnEnable()
    {
        gameManagerVm?.RequestPause();
    }
    private void OnDisable()
    {
        gameManagerVm?.RequestResume();
    }
    public void ResumeButton()
    {
        uiController?.CloseMenu();
    }
    public void QuitButton()
    {
        gameManagerVm?.RequestQuit();
    }
}
