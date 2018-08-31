using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUtilities : MonoBehaviour {

    public void RestartLevel() {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void QuitToMenu() {
        if(LevelManager.Instance == null) { return; }
		LevelManager.Instance.LoadScene("MainMenu");
    }
}