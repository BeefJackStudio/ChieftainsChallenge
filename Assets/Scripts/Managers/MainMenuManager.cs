using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    public AudioClip songToPlay;

    private void Start() {
        SoundMusicPlayer.Instance.PlayMusic(songToPlay);
    }

}
