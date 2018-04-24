using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsPlayer : MonoBehaviourSingleton<SoundEffectsPlayer> {

    private AudioSource m_AudioSource;

    private void Awake() {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float f) {
        m_AudioSource.volume = f;
    }

    public void PlaySFX(AudioClip clip, float sfxVolumeScale = 1) {
        m_AudioSource.PlayOneShot(clip, sfxVolumeScale);
    }

    public void PlaySFX(SoundEffectCollection collection) {
        collection.Play(m_AudioSource);
    }
}