using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotButton : MonoBehaviour {

    public GameLevelCollection level;

    public void StartOneShot() {
        LevelManager.Instance.LoadLevel(level.levels[0]);
    }
}
