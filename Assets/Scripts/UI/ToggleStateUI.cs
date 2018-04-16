using UnityEngine;
using System.Collections;

public class ToggleStateUI : MonoBehaviour {

    void OnEnable() {
        if (GameManager.Instance.gameType != GameManager.GameType.Cannon)
            gameObject.SetActive(false);
    }
}
