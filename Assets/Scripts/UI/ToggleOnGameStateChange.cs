using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleOnGameStateChange : MonoBehaviour {

    public GameObject toggleItem;

    public List<GameManager.GameState> activateOnState;
    public List<GameManager.GameState> deactivateOnState;

    //------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        foreach (GameManager.GameState thisState in activateOnState) {
            if (thisState == currentState)
                toggleItem.SetActive(true);
        }

        foreach (GameManager.GameState thisState in deactivateOnState) {
            if (thisState == currentState)
                toggleItem.SetActive(false);
        }
    }
}
