using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationUnlockData {

    public GameObject obj;
    public int category;
    public int listIndex;

    public void Unlock() {
        SetUnlockState(true);
    }

    public void Lock() {
        SetUnlockState(false);
    }

    private void SetUnlockState(bool b) {
        SaveData data = SaveDataManager.Instance.data;

        if (category == 0) {
            data.masksUnlocked[listIndex] = b;
        } else if (category == 1) {
            data.ballsUnlocked[listIndex] = b;
        } else if (category == 2) {
            data.skinsUnlocked[listIndex] = b;
        } else if (category == 3) {
            data.particlesUnlocked[listIndex] = b;
        }
    }

}
