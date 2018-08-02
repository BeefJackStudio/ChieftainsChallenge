using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUtilities : MonoBehaviour {

    public void RestartLevel() {
        if(SaveDataManager.Instance.data.currentLives == 0) {
            VideoAdMenu.Instance.gameObject.SetActive(true);
        } else {
            LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
        }
    }

    public void QuitToMenu() {
        if(LevelManager.Instance == null) { return; }
		LevelManager.Instance.LoadScene("MainMenu");
    }
}