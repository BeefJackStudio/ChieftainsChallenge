using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

    public GameObject MainMenuPanel;
    public GameObject MapPanel;

    public void onBackClicked() {
        MapPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void LoadMainMenu() {
        LevelManager.LoadLevel("MainMenu");
    }

}
