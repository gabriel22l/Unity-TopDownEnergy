using System;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private ApplicationController applicationController;

    private void Awake()
    {
        if (applicationController == null && !TryGetComponent(out applicationController))
        {
            Debug.LogWarning("ApplicationController not found");
        }
    }
    public void LoadGameScene()
    {
        applicationController?.LoadGameScene();
    }
    public void Quit()
    {
        applicationController?.QuitApplication();
    }
}
