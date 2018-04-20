using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Slider3D masterVolumeSlider;
    public Slider3D musicVolumeSlider;
    public Slider3D sfxVolumeSlider;

    public void Start() {
        masterVolumeSlider.Value = SaveDataManager.Instance.data.volumeMaster;
        musicVolumeSlider.Value = SaveDataManager.Instance.data.volumeMusic;
        sfxVolumeSlider.Value = SaveDataManager.Instance.data.volumeSFX;

        masterVolumeSlider.onValueChange += OnMasterVolumeChanged;
        musicVolumeSlider.onValueChange += OnBackgroundMusicChanged;
        sfxVolumeSlider.onValueChange += OnSoundEffectVolumeChanged;
    }

    public void OnMasterVolumeChanged(float newVolume) {
        Debug.Log("MasterVolume: " + newVolume.ToString());
        SoundManager.MasterVolume = newVolume;
    }

    public void OnSoundEffectVolumeChanged(float newVolume) {
        Debug.Log("SoundEffectVolume: " + newVolume.ToString());
        SoundManager.SoundEffectVolume = newVolume;
    }

    public void OnBackgroundMusicChanged(float newVolume) {
        Debug.Log("BackgroundMusicVolume: " + newVolume.ToString());
        SoundManager.BackgroundMusicVolume = newVolume;
    }
}
