using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnsLeftHUD : MonoBehaviourSingleton<TurnsLeftHUD> {

    public AnimationCurve curve;
    public float duration = 2.5f;

    private RectTransform m_RectTransform;
    private TextMeshProUGUI m_Text;
    private Coroutine m_Routine;

    private void Awake() {
        m_RectTransform = GetComponent<RectTransform>();
        m_Text = GetComponent<TextMeshProUGUI>();

        //m_RectTransform.localScale = Vector2.zero;
    }

    public void SetShotsLeft(int shotsLeft) {
        m_Text.text = "Par: " + shotsLeft;
    }

    public void StartSequence(int nextStar, int shotsLeft) {
        if (m_Routine != null) StopCoroutine(m_Routine);

        m_Text.text = shotsLeft + " shot" + (shotsLeft == 1 ? "" : "s") + " remaining until\n";
        if (nextStar == 0) {
            m_Text.text += "level is failed!";
        } else {
            m_Text.text += "star score is reduced to " + nextStar;
        }

        m_Routine = StartCoroutine(TurnsLeftSequence());
    }

    private IEnumerator TurnsLeftSequence() {
        float startTime = Time.time;
        float currentTime = Time.time;
        while(currentTime < startTime + duration) {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, startTime, startTime + duration);
            m_RectTransform.localScale = Vector2.one * curve.Evaluate((currentTime - startTime) / duration);
            yield return null;
        }
        m_Routine = null;
    }
}
