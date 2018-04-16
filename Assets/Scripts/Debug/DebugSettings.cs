using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugSettings : MonoBehaviour {

    public void OnDebugClicked() {
        SceneManager.LoadScene("DebugSettings");
    }

    public void OnBackClicked() {
        SceneManager.LoadScene("MainMenu");
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
