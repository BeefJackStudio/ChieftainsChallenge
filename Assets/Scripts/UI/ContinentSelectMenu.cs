using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentSelectMenu : MonoBehaviourSingleton<ContinentSelectMenu> {

    public Transform levelSelectView;

    public void OnContinentSelect(LevelSelectContinent continent, bool isLocked) {
        if (isLocked) return;

        UI3D.Instance.MoveCameraTo(levelSelectView);
    }
}
