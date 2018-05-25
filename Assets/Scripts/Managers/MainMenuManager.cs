using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    public AudioClip songToPlay;

    [Header("Menu locations")]
    public Transform mainMenuView;
    public Transform mapView;
    public Transform optionsView;
    public Transform levelSelectView;

    [Header("Unique cases")]
    public GameObject boxOverlay;

    private void Start() {
        SaveDataManager.Instance.Save();
        SoundMusicPlayer.Instance.PlayMusic(songToPlay);
        LivesIndicator.Instance.Show();
    }

    //Android back button behaviour
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && UI3D.Instance != null) {
            string curLoc = UI3D.Instance.GetCurrentLocation();

            if(curLoc == mainMenuView.name) {
                Application.Quit();
            }
            if(curLoc == mapView.name) {
                UI3D.Instance.MoveCameraTo(mainMenuView);
                boxOverlay.SetActive(true);
            }
            if(curLoc == optionsView.name) {
                UI3D.Instance.MoveCameraTo(mainMenuView);
            }
            if(curLoc == levelSelectView.name) {
                UI3D.Instance.MoveCameraTo(mapView);
                boxOverlay.SetActive(false);
            }
        }
    }
}
