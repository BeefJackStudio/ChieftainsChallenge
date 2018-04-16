using UnityEngine;
using System.Collections;

public class CrowedSFX : MonoBehaviour {
    public AudioClip[] happy;
    public AudioClip[] sad;

    AudioSource[] audioSources;

    float playDelay;

    // Use this for initialization
    void Start() {
        audioSources = GetComponents<AudioSource>();
        playDelay = 0;
    }

    // Update is called once per frame
    void Update() {
        //UpdateSounds(happy,0.2f,0.5f);
        UpdateSounds(sad, 0.5f, 1.0f);
    }

    void UpdateSounds(AudioClip[] clips, float minDel, float maxDel) {
        playDelay -= Time.unscaledDeltaTime;
        if (playDelay <= 0) {
            for (int index = 0; index < audioSources.Length; index++) {
                if (!audioSources[index].isPlaying) {
                    SoundManager.playSFXOneShot(audioSources[index], clips[Random.Range(0, clips.Length)]);
                    playDelay = Random.Range(minDel, maxDel);
                }
            }
        }
    }
}
