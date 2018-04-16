using UnityEngine;
using System.Collections;
using System;

public class BallCamera : ResettableObject {
    public enum FVState {
        NoTouch,
        Touch,
        ReleaseTouch
    }

    public FVState fvstate;

    public enum CameraStat {
        PreGame,
        StartGame,
        FreeCam,
        TrackBall
    }

    public Camera cam { get; set; }

    public Transform PanningStartPosition;
    public bool panCameraAtStart;

    public Transform target;
    public float xEdgeOffset = 7;
    public float yEdgeOffset = 5;
    public float groundOffset = 9.5f;
    public bool Minimap;
    private bool minimapobject = false;
    public Camera minimapCamera;
    public BallCamera maincam;

    public Vector3 targetPos;

    public float targetFOV;

    public CameraStat m_state;

    public Vector2 positionDiff;

    public Vector3 lastTouch;
    public Vector3 thisTouch;

    private Vector3 camAcceleration = Vector3.zero;
    private float drag = 0.85f;
    private float camSpeed = 2.0f;
    private float camTouchSpeed = 9;
    float pinchEndDelya;

    float velocityDelay;

    Vector2[] touchdelat;
    int tdIndex;
    Vector2 avDelta;
    public AnimationCurve cameraDropOff;
    float time;

    private bool holdingObject;

    public bool HoldingObject {
        get {
            return holdingObject;
        }
        set {
            holdingObject = value;
            if (!value) {

                TouchStart();
                if (Minimap == true) {
                    m_state = CameraStat.FreeCam;
                    minimapobject = true;
                }

            }
        }
    }

    void OnEnable() {
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;

        cam = GetComponent<Camera>();

        targetPos = transform.position;
        targetFOV = GetComponent<Camera>().fieldOfView;

        InputManager.inputMoved += TouchInput;
        InputManager.inputStart += TouchStart;

        InputManager.inputFinished += TouchEnd;
        if (panCameraAtStart && PanningStartPosition != null) {
            m_state = CameraStat.PreGame;
            cam.transform.position = PanningStartPosition.position;
        } else {
            m_state = CameraStat.StartGame;
        }
        velocityDelay = 0;
        touchdelat = new Vector2[8];
        tdIndex = 0;

    }

    void OnDisable() {
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
    }

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.preGame:
                State = CameraStat.PreGame;
                break;
            case GameManager.GameState.FreeView:
                State = CameraStat.FreeCam;
                positionDiff = Vector2.zero;
                targetFOV = 65;
                break;
            case GameManager.GameState.FollowBall:
                State = CameraStat.TrackBall;
                targetFOV = 85;
                break;
            case GameManager.GameState.WaitForShot:
                State = CameraStat.TrackBall;
                break;
        }
    }

    public CameraStat State {
        get {
            return m_state;
        }

        set {
            if (m_state == CameraStat.TrackBall)
                Debug.Log("Track Ball");
            m_state = value;
            if (value == CameraStat.FreeCam) {
                lastTouch = thisTouch;

            }
        }
    }

    public void onMiniMapTouch() {
        transform.position = new Vector3(minimapCamera.transform.position.x, minimapCamera.transform.position.y, transform.position.z);
        lastTouch.z = -transform.position.z;
        thisTouch.z = -transform.position.z;
        Vector3 p1 = minimapCamera.WorldToScreenPoint(lastTouch);
        Vector3 p2 = minimapCamera.WorldToScreenPoint(thisTouch);

        Vector3 inputForce = p2 - p1;
        cam.transform.position = new Vector3(cam.transform.position.x + inputForce.x, cam.transform.position.y + inputForce.y, cam.transform.position.z);

        lastTouch = thisTouch;

    }

    public bool TouchingMinimap { get; private set; }

    void LateUpdate() {
        switch (m_state) {
            case CameraStat.PreGame: {
                    PanCamera();
                    break;
                }
            case CameraStat.FreeCam: {
                    FreeMoveCamera();
                    break;
                }
            case CameraStat.TrackBall: {
                    TrackBall();
                    break;
                }
        }
    }

    private void PanCamera() {
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, initialPosition.position, camSpeed);
        if (cam.transform.position == initialPosition.position) {
            if (Minimap == true) {
                m_state = CameraStat.TrackBall;
            } else {
                m_state = CameraStat.StartGame;
            }
        }
    }

    void TouchStart() {

        lastTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);

        for (int index = 0; index < 8; index++) {
            touchdelat[index] = Vector2.zero;
        }
        tdIndex = 0;
        fvstate = FVState.Touch;
        camAcceleration = Vector2.zero;
    }
    void TouchInput() {
        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);

        //loop around the rindbuffer adding the deltas
        touchdelat[tdIndex] = InputManager.positionDelta;
        tdIndex++;
        tdIndex &= 7;

    }

    void TouchEnd() {
        avDelta = Vector2.zero;

        for (int index = 0; index < 8; index++) {
            avDelta += touchdelat[index];
        }
        avDelta /= 8;

        fvstate = FVState.ReleaseTouch;
        time = 0;
    }

    void FreeMoveCamera() {
        if (!holdingObject) {
            if (fvstate == FVState.Touch) {
                if (Input.touchCount == 2) {

                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


                    // Otherwise change the field of view based on the change in distance between the touches.
                    targetFOV += deltaMagnitudeDiff * 10;
                    targetFOV = Mathf.Clamp(targetFOV, 40, 100);

                    cam.fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);
                    pinchEndDelya = 0.1f;
                } else {
                    if (pinchEndDelya > 0) {
                        pinchEndDelya -= Time.deltaTime;

                    } else {
                        cam.fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);
                        lastTouch.z = -transform.position.z;
                        thisTouch.z = -transform.position.z;
                        Vector3 p1 = cam.ScreenToWorldPoint(lastTouch);
                        Vector3 p2 = cam.ScreenToWorldPoint(thisTouch);

                        Vector3 inputForce = p1 - p2;

                        camAcceleration += inputForce * camTouchSpeed;
                        camAcceleration *= drag;


                        transform.position += camAcceleration * Time.deltaTime;

                        positionDiff = Vector2.zero;
                        velocityDelay = 2;
                        lastTouch = thisTouch;
                    }
                }
            } else if (fvstate == FVState.ReleaseTouch) {
                float delta = cameraDropOff.Evaluate(time);
                Vector3 pos = transform.position;
                pos.x += (camAcceleration.x * Time.deltaTime) * delta;
                pos.y += (camAcceleration.y * Time.deltaTime) * delta;
                transform.position = pos;
                time += Time.deltaTime;
                if (time >= 1) {
                    fvstate = FVState.NoTouch;
                    avDelta = Vector2.zero;
                }
            } else if (fvstate == FVState.NoTouch) {
                cam.fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFOV, Time.fixedDeltaTime);
                camAcceleration = Vector2.zero;
            }

            if (Minimap == true) {
                m_state = CameraStat.TrackBall;
                minimapobject = false;
            }
        }
    }

    void TrackBall() {
        if (Minimap == true) {
            maincam.FreeMoveCamera();

        }

        velocityDelay += Time.deltaTime;

        if (target.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f) {
            velocityDelay = 0;
        }

        Vector3 pos = targetPos;
        Vector3 diff = pos - target.position;

        //keep ball framed by camera with some give
        //check y first 
        if (diff.y < -yEdgeOffset) {
            pos.y += -(diff.y + yEdgeOffset);
        } else if (diff.y > yEdgeOffset && pos.y > groundOffset) {
            pos.y -= diff.y - yEdgeOffset;

            //check were not going to go through the ground
            if (pos.y < groundOffset) {
                pos.y = groundOffset;
            }
        }
        //then check x
        pos.x = target.position.x + -xEdgeOffset;

        //now we have were we want to be
        targetPos = pos;

        //update the camera position to head toword the target position
        pos = transform.position;
        targetPos.z = pos.z;
        pos = Vector3.Lerp(pos, targetPos, Time.deltaTime * 3);

        transform.position = pos;

        //based on the velocity of the camera this will change the fov to give use zomming in and out 
        GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(GetComponent<Camera>().fieldOfView, targetFOV, 1);// Time.fixedDeltaTime);
    }


}
