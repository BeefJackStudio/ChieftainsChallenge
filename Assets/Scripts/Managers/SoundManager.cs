using UnityEngine;

/// <summary>
/// Controls sound volume, allows indiviual sounds to be played
/// </summary>
public static class SoundManager {

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
        }
    }

    public static float BackgroundMusicVolume {
        get { return SaveDataManager.Instance.data.volumeMusic; }
        set {
            SaveDataManager.Instance.data.volumeMusic = value;
        }
    }

}
