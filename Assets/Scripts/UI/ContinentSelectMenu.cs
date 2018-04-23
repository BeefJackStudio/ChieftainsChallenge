using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentSelectMenu : MonoBehaviourSingleton<ContinentSelectMenu> {

    public Transform levelSelectView;

    public void OnContinentSelect(ContinentSelectButton continent, bool isLocked) {
        if (isLocked) return;

        UI3D.Instance.MoveCameraTo(levelSelectView);

        LevelSelectMenu.Instance.CreateButtons(continent.levelSet);
    }
}
