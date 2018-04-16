using UnityEngine;
using System.Collections;

public delegate void EventHandler();

public class InputManager : MonoBehaviour {
    public static event EventHandler inputFinished;
    public static event EventHandler inputMoved;
    public static event EventHandler inputStart;

    public enum InputState {
        NoTouch,
        StartTouch,
        EndTouch
    };

    InputState state;
    public static Vector2 positionStart;
    public static Vector2 positionEnd;
    public static Vector2 positionLast;
    public static Vector2 positionRaw;
    public static Vector2 positionDelta;
    public static float touchTime;

    public static Vector2 screenScale;

    // Use this for initialization
    void Start() {
        screenScale.x = 1024.0f / (float)Screen.width;
        screenScale.y = 640.0f / (float)Screen.height;
    }

    public static Vector2 InputToScreenPoint(Vector2 input) {
        Vector2 screen = new Vector2(input.x / screenScale.x, input.y / screenScale.y);
        return screen;
    }

    public static Vector2 ScreenPointToInput(Vector2 point) {
        Vector2 input = new Vector2(point.x * screenScale.x, point.y * screenScale.y);
        return input;
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case InputState.NoTouch: {
#if UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0)) {
                        state = InputState.StartTouch;
                        positionStart = Input.mousePosition;
                        positionStart.x *= screenScale.x;
                        positionStart.y *= screenScale.y;
                        positionDelta = Vector2.zero;
                        positionLast = positionStart;
                        positionRaw = Input.mousePosition;
                        touchTime = 0;
                        if (inputStart != null)
                            inputStart.Invoke();
                    }

#else
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    state = InputState.StartTouch;
                    positionStart = Input.GetTouch(0).position;
                    positionStart.x *= screenScale.x;
                    positionStart.y *= screenScale.y;
                    positionDelta = Vector2.zero;
                    positionLast = positionStart;
                    positionRaw = Input.GetTouch(0).position;

                    touchTime = 0;
                    if(inputStart!= null)
                        inputStart.Invoke();
                }
                
#endif
                    break;
                }
            case InputState.StartTouch: {
#if UNITY_EDITOR
                    Vector2 newPos = Input.mousePosition;
                    newPos.x *= screenScale.x;
                    newPos.y *= screenScale.y;

                    positionDelta = newPos - positionLast;
                    if (Input.GetMouseButtonUp(0)) {
                        state = InputState.EndTouch;
                        positionEnd = newPos;
                    } else if (positionDelta.magnitude > 0) {
                        if (inputMoved != null)
                            inputMoved.Invoke();
                    }
                    positionLast = newPos;
                    positionRaw = Input.mousePosition;

#else
                Vector2 newPos = Input.GetTouch(0).position;
                newPos.x *= screenScale.x;
                newPos.y *= screenScale.y;
                positionDelta = newPos - positionLast;
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    state = InputState.EndTouch;
                    positionEnd = newPos;
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (inputMoved != null)
                        inputMoved.Invoke();
                }
                touchTime += Input.GetTouch(0).deltaTime;
                positionLast = newPos;
                positionRaw = Input.GetTouch(0).position;
#endif
                    break;
                }
            case InputState.EndTouch: {
                    positionDelta = Vector2.zero;
                    if (inputMoved != null)
                        inputMoved.Invoke();
                    if (inputFinished != null)
                        inputFinished.Invoke();
                    state = InputState.NoTouch;
                    break;
                }
        }
    }
}
