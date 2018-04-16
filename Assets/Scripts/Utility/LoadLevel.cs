using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

    public void LoadThisLevel(string level) {
        Application.LoadLevel(level);
    }
}
