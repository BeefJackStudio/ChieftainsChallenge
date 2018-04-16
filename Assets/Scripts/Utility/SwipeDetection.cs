using System;
using UnityEngine;

public class SwipeDetection : MonoBehaviourSingleton<SwipeDetection> {

    public static event Action<SwipeDirection, Vector2> OnSwipe;

    private bool m_IsSwiping = false;
    private bool m_EventSent = false;
    private Vector2 m_LastPosition;
    private float m_MinimumMovement = 0.0f;

    void Start() {
        m_MinimumMovement = Screen.height * 0.1f;
    }

    void Update() {
        if (Input.touchCount == 0)
            return;

        if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0) {
            if (m_IsSwiping == false) {
                m_IsSwiping = true;
                m_LastPosition = Input.GetTouch(0).position;
                return;
            } else {
                if (!m_EventSent && OnSwipe != null) {
                    Vector2 direction = Input.GetTouch(0).position - m_LastPosition;
                    Vector2 delta = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

                    if (Mathf.Abs(delta.x) >= m_MinimumMovement) {
                        if (delta.x < 0) OnSwipe(SwipeDirection.Left, delta);
                        else OnSwipe(SwipeDirection.Right, delta);
                        m_EventSent = true;
                    } else if (Mathf.Abs(delta.y) >= m_MinimumMovement) {
                        if (delta.y < 0) OnSwipe(SwipeDirection.Down, delta);
                        else OnSwipe(SwipeDirection.Up, delta);
                        m_EventSent = true;
                    }
                }
            }
        } else {
            m_IsSwiping = false;
            m_EventSent = false;
        }
    }

    public enum SwipeDirection {
        Up,
        Down,
        Right,
        Left
    }
}
