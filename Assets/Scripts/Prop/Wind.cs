using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wind : MonoBehaviour {
    public RectTransform windArrow;
    public Text mphText;

    AreaEffector2D af;
    float wrAngle;

    public float pitchDelta;
    public float pitchTime;

    AudioSource audioSource;

    //------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;

        af = GetComponent<AreaEffector2D>();
        ChangeWindDirection();

        audioSource = GetComponent<AudioSource>();

        ChangeAudio();
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        if (currentState == GameManager.GameState.FreeView)
            ChangeWindDirection();
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    void ChangeAudio() {
        float target = Random.Range(0.8f, 1.2f);
        pitchTime = Random.Range(0.5f, 1.5f);

        float diff = target - audioSource.pitch;
        pitchDelta = diff / pitchTime;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    void UpdateAudio() {
        if (audioSource) {
            float dt = Time.deltaTime;
            pitchTime -= dt;
            audioSource.pitch += pitchDelta * dt;
            if (pitchTime <= 0) {
                ChangeAudio();
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    void Update() {
        UpdateAudio();

        float afAngle = af.forceAngle;
        wrAngle = Mathf.LerpAngle(wrAngle, afAngle, Time.deltaTime);
        Vector3 rot = windArrow.eulerAngles;
        rot.z = wrAngle - 90;
        windArrow.eulerAngles = rot;

        mphText.text = "mph: " + (int)(af.forceMagnitude * 100);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    public void ChangeWindDirection() {
        if (Random.Range(0, 100) < 50) {
            af.forceAngle = 0;
        } else {
            af.forceAngle = 180;
        }

        float speed = Random.Range(0.01f, 0.9f);
        af.forceMagnitude = speed;
        GetComponent<AudioSource>().volume = SoundManager.SoundEffectVolume * speed;
    }
}
