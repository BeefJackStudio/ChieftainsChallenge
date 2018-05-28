using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesIndicator : MonoBehaviourSingleton<LivesIndicator> {

    public RectTransform[] livesImages;
    public AnimationCurve animationCurve;
    public AnimationCurve animationCurveRotation;

    private RectTransform m_RectTransform;
    private List<LifeObject> m_LifeObjects = new List<LifeObject>();
    private Vector2 m_StartPosition;
    private ObjectLerper m_Lerper;

    private void Awake() {
        foreach(RectTransform li in livesImages) {
            m_LifeObjects.Add(new LifeObject(li.GetComponent<Image>()));
        }

        m_RectTransform = GetComponent<RectTransform>();
        m_Lerper = GetComponent<ObjectLerper>();
        m_StartPosition = m_RectTransform.anchoredPosition;

        m_RectTransform.anchoredPosition += new Vector2(0, 200);
        m_Lerper.targetPosition = m_RectTransform.anchoredPosition;
    }

    public void Show() {
        ObjectLerper lerper = GetComponent<ObjectLerper>();
        lerper.targetPosition = m_StartPosition;

        TimeUtilities.ExecuteAfterDelay(() => {
            SetLivesCount(SaveDataManager.Instance.data.currentLives);
        }, 1, this);
    }

    public void SetLivesCount(int livesCount) {
        int count = 0;
        foreach (LifeObject lo in m_LifeObjects) {
            bool hasLife = (count + 1) <= livesCount;
            TimeUtilities.ExecuteAfterDelay(() => { if (hasLife) lo.Enable(); else lo.Disable(); }, count * 0.3f, this);
            count++;
        }
    }

    [ContextMenu("Enable")]
    private void EnableAll() {
        int count = 0;
        foreach(LifeObject lo in m_LifeObjects) {
            TimeUtilities.ExecuteAfterDelay(lo.Enable, count * 0.3f, this);
            count++;
        }
    }


    [ContextMenu("Disable")]
    private void DisableAll() {
        foreach (LifeObject lo in m_LifeObjects) {
            lo.Disable();
        }
    }

    private class LifeObject
    {
        private Image m_Image;
        private float m_SwitchValue = 0;
        private Coroutine m_SwitchRoutine = null;

        public LifeObject(Image image) {
            m_Image = image;
        }

        public void Enable() {
            if (m_SwitchValue == 1) return;
            ClearRoutine();
            m_Image.StartCoroutine(SwitchRoutine(1));
        }

        public void Disable() {
            if (m_SwitchValue == 0) return;
            ClearRoutine();
            m_Image.StartCoroutine(SwitchRoutine(0));
        }

        private void ClearRoutine() {
            if (m_SwitchValue == 0) return;
            if (m_SwitchRoutine != null) m_Image.StopCoroutine(m_SwitchRoutine);
        }

        private IEnumerator SwitchRoutine(float target) {
            target = Mathf.Clamp01(target);
            float direction = target == 0 ? -1 : 1;
            while (!Mathf.Approximately(m_SwitchValue, target)) {
                m_SwitchValue = Mathf.Clamp01(m_SwitchValue + (direction * Time.deltaTime * 2));
                SetState(m_SwitchValue);
                yield return null;
            }
            m_SwitchRoutine = null;
        }

        private void SetState(float i) {
            float color = (i / 2) + 0.5f;
            m_Image.color = new Color(color, color, color, 1);

            float scale = LivesIndicator.Instance.animationCurve.Evaluate(i) + 1;
            m_Image.rectTransform.localScale = new Vector3(scale, scale, 1);

            float rotation = LivesIndicator.Instance.animationCurveRotation.Evaluate(i) * 360;
            m_Image.rectTransform.localEulerAngles = new Vector3(0, 0, rotation);
        }
    }
}
