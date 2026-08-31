using UnityEngine;

public class GameManagerVM
{
    private GameManager gameManager;

    public GameManagerVM(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    public void RequestPause()
    {
        gameManager?.Pause(true);
    }
    public void RequestResume()
    {
        gameManager?.Pause(false);
    }
    public void RequestQuit()
    {
        gameManager?.QuitToMainMenu();
    }
}
