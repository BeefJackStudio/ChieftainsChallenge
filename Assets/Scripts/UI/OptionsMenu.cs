using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Slider3D masterVolumeSlider;
    public Slider3D musicVolumeSlider;
    public Slider3D sfxVolumeSlider;

    public void Start() {
        if (SaveDataManager.Instance != null) {
            masterVolumeSlider.Value = SaveDataManager.Instance.data.volumeMaster;
            musicVolumeSlider.Value = SaveDataManager.Instance.data.volumeMusic;
            sfxVolumeSlider.Value = SaveDataManager.Instance.data.volumeSFX;

            masterVolumeSlider.onValueChange += OnMasterVolumeChanged;
            musicVolumeSlider.onValueChange += OnBackgroundMusicChanged;
            sfxVolumeSlider.onValueChange += OnSoundEffectVolumeChanged;
        }
    }

    public void OnMasterVolumeChanged(float newVolume) {
        SoundManager.MasterVolume = newVolume;
    }

    public void OnSoundEffectVolumeChanged(float newVolume) {
        SoundManager.SoundEffectVolume = newVolume;
    }

    public void OnBackgroundMusicChanged(float newVolume) {
        SoundManager.BackgroundMusicVolume = newVolume;
    }
}
