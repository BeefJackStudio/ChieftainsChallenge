using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour {

    private TextMeshProUGUI m_TextBox;
    private Queue<float> m_FramerateQueue = new Queue<float>();
    private float m_MaxQueueSize = 10;

    private void Awake() {
        m_TextBox = GetComponent<TextMeshProUGUI>();
    }

    void Update () {
        float currentFramerate = (1 / Time.deltaTime);
        m_FramerateQueue.Enqueue(currentFramerate);

        if(m_FramerateQueue.Count > m_MaxQueueSize) {
            m_FramerateQueue.Dequeue();
        }

        float framerate = 0;
        foreach(float f in m_FramerateQueue) {
            framerate += f;
        }
        framerate /= m_FramerateQueue.Count;

        m_TextBox.text = ""+ (int)framerate;
	}
}
