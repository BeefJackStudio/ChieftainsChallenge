using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public static string HoleInOneGame = "Hole In One";
    public static string ChalangeGame = "Level 1";

    public GameObject MainMenuPanel;
    public GameObject OptionsPanel;
    public GameObject MapPanel;
    public GameObject MasksPanel;
    public GameObject EquipmentPanel;

    public void StartGame() {
        LevelManager.LoadLevel(ChalangeGame);
    }

    public void StartHoleInOneGame() {
        EquipmentPanel.GetComponent<EquipmentSelect>().levelToLoad = HoleInOneGame;//levelToLoad
        EquipmentPanel.SetActive(true);
    }

    public void OnOptionsClicked() {
        OptionsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OnMapClicked() {
        MapPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OnMasksClicked() {
        MasksPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OnLoadWorldMap() {
        LevelManager.LoadLevel("WorldMap");
    }
}
