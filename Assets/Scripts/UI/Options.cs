using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour {

    public GameObject MainMenuPanel;
    public GameObject OptionsPanel;

    public Slider MasterVolume;
    public Slider BGMVolume;
    public Slider SFXVolume;

    public void Start() {
        MasterVolume.value = SaveDataManager.Instance.data.volumeMaster;
        BGMVolume.value = SaveDataManager.Instance.data.volumeMusic;
        SFXVolume.value = SaveDataManager.Instance.data.volumeSFX;
    }

    public void onMasterVolumeChanged(float newVolume) {
        Debug.Log("MasterVolume: " + newVolume.ToString());
        SoundManager.MasterVolume = newVolume;
    }

    public void onSoundEffectVolumeChanged(float newVolume) {
        Debug.Log("SoundEffectVolume: " + newVolume.ToString());
        SoundManager.SoundEffectVolume = newVolume;


    }

    public void onBackgroundMusicChanged(float newVolume) {
        Debug.Log("BackgroundMusicVolume: " + newVolume.ToString());
        SoundManager.BackgroundMusicVolume = newVolume;
    }

    public void onBackClicked() {
        OptionsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
}
