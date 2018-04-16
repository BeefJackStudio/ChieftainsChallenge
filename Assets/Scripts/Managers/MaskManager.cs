using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MaskManager : MonoBehaviour {

    public List<PowerMask> mask;
    public PowerMask activeMask { get; set; }
    public PowerMask defaultMask;

    public List<GameObject> UIMaskes;

    public Button IconButton;

    public int maskIndex = -1;
    public RectTransform MaskButtons;

    public AudioClip maskSelectSFX;

    public bool maskActive { get; set; }

    public Image powerBar;
    public Image subBar;
    public RectTransform startOfPowerBar;

    public SpriteRenderer playerMask;
    public SpriteRenderer playerMaskOutline;

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;
        NotificationsManager.OnGameStateStay += NotificationsManager_OnGameStateStay;
        NotificationsManager.OnInputStateChange += NotificationsManager_OnInputStateChange;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
        NotificationsManager.OnGameStateStay -= NotificationsManager_OnGameStateStay;
        NotificationsManager.OnInputStateChange -= NotificationsManager_OnInputStateChange;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType currentGameType, int coursePar) {
        InitialiseMasks();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.FreeView:
                ResetMask();
                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateStay(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.WaitForShot:
                MaskOn();
                break;
            case GameManager.GameState.FollowBall:
                MaskOff();
                SetIconButton();
                break;
            case GameManager.GameState.GameFinished:
                MaskOff();
                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnInputStateChange(GameManager.InputState currentState, Vector2 backPos) {
        switch (currentState) {
            case GameManager.InputState.PlayerSelected: {
                    if (activeMask) {
                        activeMask.Use();
                    } else {
                        maskActive = false;
                    }
                }
                break;

            case GameManager.InputState.SwingBack: {
                    if (activeMask) {
                        // TODO investigate why TestPoint not working correctly (bypass for moment)
                        //if (activeMask.TestPoint(backPos))
                        {
                            activeMask.UseActive();
                            maskActive = true;
                        }
                    }
                }
                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void InitialiseMasks() {
        maskIndex = -1;

        if (EquipmentSelection.Mask1 != null) {
            mask = new List<PowerMask>();

            mask.Add(EquipmentSelection.Mask1);
            mask.Add(EquipmentSelection.Mask2);
            mask.Add(EquipmentSelection.Mask3);
        }

        for (int index = 0; index < mask.Count; index++) {
            mask[index].maskManager = this;
            mask[index].Init(index);
        }

        defaultMask.maskManager = this;
        defaultMask.Init(-1);
        defaultMask.Use();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void MaskOff() {
        Vector3 pos = MaskButtons.localPosition;
        pos.y = Mathf.Lerp(pos.y, -400.0f, Time.deltaTime * 3);
        MaskButtons.localPosition = pos;

        if (powerBar.sprite != null) {
            Color col = powerBar.color;
            col.a = Mathf.Lerp(col.a, 0, Time.deltaTime);
            powerBar.color = col;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void MaskOn() {
        Vector3 pos = MaskButtons.localPosition;
        pos.y = Mathf.Lerp(pos.y, -200.0f, Time.deltaTime * 3);
        MaskButtons.localPosition = pos;

        if (powerBar.sprite != null) {
            Color col = powerBar.color;
            col.a = Mathf.Lerp(col.a, 1, Time.deltaTime);
            powerBar.color = col;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void ResetMask() {
        if (maskIndex != -1) {
            activeMask.DeactivateMask();
            Destroy(activeMask);
        }
        maskIndex = -1;
        defaultMask.Use();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public void SelectPowerMask() {
        SoundManager.playSFXOneShot(GetComponent<AudioSource>(), maskSelectSFX);
        maskIndex = 0;
        if (activeMask) {
            Destroy(activeMask);
        }
        activeMask = Instantiate(mask[maskIndex]);
        activeMask.maskManager = this;
        activeMask.Use();

        powerBar.color = new Color(1, 1, 1, 0);
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public void SelectPitchMask() {
        SoundManager.playSFXOneShot(GetComponent<AudioSource>(), maskSelectSFX);
        maskIndex = 1;
        if (activeMask) {
            Destroy(activeMask);
        }
        activeMask = Instantiate(mask[maskIndex]);
        activeMask.maskManager = this;
        activeMask.Use();

        powerBar.color = new Color(1, 1, 1, 0);
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public void SelectPutMask() {
        SoundManager.playSFXOneShot(GetComponent<AudioSource>(), maskSelectSFX);
        maskIndex = 2;
        if (activeMask) {
            Destroy(activeMask);
        }
        activeMask = Instantiate(mask[maskIndex]);
        activeMask.maskManager = this;
        activeMask.Use();

        powerBar.color = new Color(1, 1, 1, 0);
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public void ActivateMask() {
        if (maskIndex != -1) {
            activeMask.ActivateIcon();
            IconButton.gameObject.SetActive(false);
            maskActive = false;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void SetIconButton() {
        if (activeMask && maskActive) {
            if (activeMask.icon == null) {
                IconButton.gameObject.SetActive(false);
            } else {
                IconButton.gameObject.SetActive(true);
                IconButton.GetComponent<Image>().sprite = mask[maskIndex].icon;
            }
        } else {
            IconButton.gameObject.SetActive(false);
        }

        if (maskIndex != -1) {
            UIMaskes[maskIndex].GetComponent<Button>().interactable = false;
        }
    }

}
