using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelUtilities))]
public class PauseMenu : MonoBehaviour {

    private LevelUtilities m_LevelUtilities;

    private void Awake() {
        m_LevelUtilities = GetComponent<LevelUtilities>();
    }

    public void RestartLevel() {
        m_LevelUtilities.RestartLevel();
    }

    public void QuitLevel() {
        m_LevelUtilities.QuitToMenu();
    }
}
