using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    public AudioClip songToPlay;

    private void Start() {
        SaveDataManager.Instance.Save();
        SoundMusicPlayer.Instance.PlayMusic(songToPlay);
    }

}
