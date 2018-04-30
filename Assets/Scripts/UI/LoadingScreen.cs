using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviourSingleton<LoadingScreen> {

    public RectTransform fadeImage;
    public CanvasGroup content;
    public AnimationCurve animationCurve;
    public float transitionTime = 0;

    private Coroutine m_Routine;
    private bool m_IsShown = false;
    public bool IsShown { get { return m_IsShown; } }

    public void Initialize() {
        gameObject.SetActive(false);
    }

    public void Show(Action onComplete) {
        if (m_IsShown) return;
        m_IsShown = true;
        AnimateTransition(true, onComplete);
    }
    
    public void Hide(Action onComplete) {
        if (!m_IsShown) return;
        m_IsShown = false;
        AnimateTransition(false, onComplete);
    }

    private void AnimateTransition(bool state, Action onComplete) {
        if (m_Routine != null) StopCoroutine(m_Routine);

        gameObject.SetActive(true);

        float imageWidth = fadeImage.rect.width;
        float startX = state ? -imageWidth : 0;
        float endX = state ? 0 : imageWidth;

        float startTransparency = state ? 0 : 1;
        float endTransparency = state ? 1 : 0;

        m_Routine = StartCoroutine(TransitionRoutine(startX * fadeImage.localScale.x * 0.8f, endX * fadeImage.localScale.x * 0.8f, startTransparency, endTransparency, onComplete));
    }

    private IEnumerator TransitionRoutine(float startX, float endX, float startTransparency, float endTransparency, Action onComplete) {
        float startTime = Time.time;
        Vector2 startPos = new Vector2(startX, fadeImage.anchoredPosition.y);
        Vector2 endPos = new Vector2(endX, fadeImage.anchoredPosition.y);

        float normalizedTime = 0;
        while (normalizedTime != 1) {
            normalizedTime = MathUtilities.GetNormalizedTime(startTime, transitionTime, Time.time);
            float curveValue = animationCurve.Evaluate(normalizedTime);
            fadeImage.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);
            content.alpha = Mathf.Lerp(startTransparency, endTransparency, curveValue);
            yield return null;
        }

        onComplete();
        m_Routine = null;

        //Means it's a hide action
        if(startX == 0) {
            gameObject.SetActive(false);
        }

        yield return null;
    }

}
