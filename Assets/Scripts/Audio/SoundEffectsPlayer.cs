using UnityEngine;
using System.Collections;

public class SoundEffectsPlayer : MonoBehaviourSingleton<SoundEffectsPlayer> {

    private AudioSource m_AudioSource;

    public AudioClip ButtonPressSound;
    public AudioClip MaskCollision;

    private void Awake() {
        m_AudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void onButtonPressed() {
        PlaySFX(ButtonPressSound);
    }

    public void onMaskCollision() {
        PlaySFX(MaskCollision);
    }

    public void PlaySFX(AudioClip clip, float sfxVolumeScale = 1) {
        m_AudioSource.volume = SaveDataManager.Instance.data.volumeSFX;
        m_AudioSource.PlayOneShot(clip, sfxVolumeScale);
    }
}
