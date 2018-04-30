using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUtilities : MonoBehaviour {

    [ReadOnly]  public int DefaultTimeScale = 1;

    public void PauseGame() {
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Time.timeScale = 1;
    }

    public void RestartLevel() {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void QuitToMenu() {
        if(LevelManager.Instance == null) { return; }
		LevelManager.Instance.LoadScene("MainMenu");
    }
}