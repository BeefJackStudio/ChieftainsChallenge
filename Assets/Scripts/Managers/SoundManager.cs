using UnityEngine;

/// <summary>
/// Controls sound volume
/// </summary>
public static class SoundManager {

    public static float MasterVolume {
        get { return SaveDataManager.Instance.data.volumeMaster; }
        set {
            SaveDataManager.Instance.data.volumeMaster = value;
            SoundMusicPlayer.Instance.SetVolume(BackgroundMusicVolume);
        }
    }

    public static float SoundEffectVolume {
        get { return SaveDataManager.Instance.data.volumeSFX * MasterVolume; }
        set {
            SaveDataManager.Instance.data.volumeSFX = value;
            SoundEffectsPlayer.Instance.SetVolume(SoundEffectVolume);
        }
    }

    public static float BackgroundMusicVolume {
        get { return SaveDataManager.Instance.data.volumeMusic * MasterVolume * 0.4f; }
        set {
            SaveDataManager.Instance.data.volumeMusic = value;
            SoundMusicPlayer.Instance.SetVolume(BackgroundMusicVolume);
        }
    }

}
