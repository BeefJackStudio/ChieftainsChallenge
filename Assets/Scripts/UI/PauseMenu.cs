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
        TrySubstractLife(true);
        m_LevelUtilities.RestartLevel();
    }

    public void QuitLevel() {
        TrySubstractLife(false);
        m_LevelUtilities.QuitToMenu();
    }

    private void TrySubstractLife(bool hideAfterwards) {
        if (LevelInstance.Instance.currentShot == 1) return;
        SaveDataManager.Instance.ModifyLifeCount(-1);

        if (hideAfterwards) LivesIndicator.Instance.Show(4, false);
        else LivesIndicator.Instance.Show(false);
    }
}
