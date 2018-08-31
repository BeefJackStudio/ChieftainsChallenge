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

    [Header("Misc")]
    public CharacterSpriteHandler mainMenuCharacter;
    public GameBall mainMenuBall;

    private void Start() {
        SaveDataManager.Instance.Save();
        SoundMusicPlayer.Instance.PlayMusic(songToPlay);

        ReloadCharacter();
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

    [ContextMenu("Reload characters")]
    public void ReloadCharacter() {
        mainMenuCharacter.ApplyMask(CustomizationSelected.GetMaskType(Random.Range(0, 4)).Obj);
        Vector3 ballPos = mainMenuBall.transform.position;
        Destroy(mainMenuBall.gameObject);

        mainMenuBall = Instantiate(CustomizationSelected.GetBallType(Random.Range(0, 4)).Obj);
        mainMenuBall.isInGame = false;
        mainMenuBall.GetComponent<Rigidbody2D>().isKinematic = true;
        mainMenuBall.transform.position = ballPos;
    }
}
