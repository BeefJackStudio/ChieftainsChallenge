using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    public enum PauseState {
        Waiting,
        PauseComingOn,
        PauseOn,
        PauseGoingOff
    };
    public RectTransform pauseMenu;
    public PauseState state;
    float velocity;

    public GameObject pMenu;
    public GameObject egMenu;

    void Update() {
        switch (state) {
            case PauseState.Waiting: {
                    break;
                }
            case PauseState.PauseComingOn: {
                    velocity -= 30f * Time.unscaledDeltaTime;
                    velocity = Mathf.Clamp(velocity, -1225, 1225);
                    Vector2 pos = pauseMenu.anchoredPosition;
                    pos.y += velocity;
                    if (pos.y <= 0) {
                        velocity = -(velocity * 0.40f);
                        pos.y = 0;
                    }
                    pauseMenu.anchoredPosition = pos;
                    if (pauseMenu.anchoredPosition.y.Aprox(0, 1) && velocity.Aprox(0, .5f)) {
                        state = PauseState.PauseOn;
                    }
                    break;
                }
            case PauseState.PauseOn: {
                    break;
                }
            case PauseState.PauseGoingOff: {
                    velocity += 30f * Time.unscaledDeltaTime;
                    velocity = Mathf.Clamp(velocity, -1225, 1225);
                    Vector2 pos = pauseMenu.anchoredPosition;
                    pos.y += velocity;
                    pauseMenu.anchoredPosition = pos;
                    if (pauseMenu.anchoredPosition.y > 550) {
                        pos.y = 550;
                        pauseMenu.anchoredPosition = pos;
                        state = PauseState.Waiting;
                        Time.timeScale = 1;
                    }
                    break;
                }
        }
    }

    public void ActivatePause() {
        if (state == PauseState.Waiting) {
            state = PauseState.PauseComingOn;
            velocity = 0;
            Time.timeScale = 0;
        }
    }

    public void Resume() {
        if (state == PauseState.PauseOn) {
            state = PauseState.PauseGoingOff;
            Time.timeScale = 1;
        }
    }

    public void Quit(string level) {
        if (state == PauseState.PauseOn) {
            Time.timeScale = 1;
            Application.LoadLevel(level);
        }
    }

    public void SwitchPauseMenu() {
        pMenu.SetActive(true);
        egMenu.SetActive(false);
    }
    public void SwitchEndGame() {
        pMenu.SetActive(false);
        egMenu.SetActive(true);
    }

    public void restartCurrentLevel() {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevelName);
    }

}
