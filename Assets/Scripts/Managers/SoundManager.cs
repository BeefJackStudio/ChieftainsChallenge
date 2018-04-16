using UnityEngine;

/// <summary>
/// Controls sound volume, allows indiviual sounds to be played
/// </summary>
public static class SoundManager {
    public delegate void ChangedEventHandler(float newVolume);
    public static event ChangedEventHandler onSFXChanged;
    public static event ChangedEventHandler onBGMChanged;

    public static float MasterVolume {
        get { return SaveDataManager.Instance.data.volumeMaster; }
        set {
            SaveDataManager.Instance.data.volumeMaster = value;
            AudioListener.volume = value;
        }
    }

    public static float SoundEffectVolume {
        get { return SaveDataManager.Instance.data.volumeSFX; }
        set {
            SaveDataManager.Instance.data.volumeSFX = value;
            if (onSFXChanged != null)
                onSFXChanged(value);
        }
    }

    public static float BackgroundMusicVolume {
        get { return SaveDataManager.Instance.data.volumeMusic; }
        set {
            SaveDataManager.Instance.data.volumeMusic = value;
            if (onBGMChanged != null)
                onBGMChanged(value);
        }
    }


    public static void pause() {
        AudioListener.pause = true;
    }

    public static void unpause() {
        AudioListener.pause = false;
    }
    public static void togglePause() {
        AudioListener.pause = !AudioListener.pause;
    }

    public static void playBGM(AudioSource source, AudioClip clip = null, float volumeMultiplyer = 1) {
        if (clip != null)
            source.clip = clip;
        source.volume = BackgroundMusicVolume * volumeMultiplyer;
        source.Play();
    }

    public static void playSFX(AudioSource source, AudioClip clip = null, float volumeMultiplyer = 1) {
        if (clip != null)
            source.clip = clip;
        source.volume = SoundEffectVolume * volumeMultiplyer;
        source.Play();
    }

    public static void playBGMOneShot(AudioSource source, AudioClip clip, float volumeMultiplyer = 1) {
        source.volume = BackgroundMusicVolume * volumeMultiplyer;
        source.PlayOneShot(clip);
    }

    public static void playSFXOneShot(AudioSource source, AudioClip clip, float volumeMultiplyer = 1) {
        source.volume = SoundEffectVolume * volumeMultiplyer;
        source.PlayOneShot(clip);
    }



}
