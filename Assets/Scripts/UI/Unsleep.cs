using UnityEngine;
using System.Collections;

public class Unsleep : MonoBehaviour {

    // Use this for initialization
    void Start() {
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update() {
        Destroy(this.gameObject);
    }
}
