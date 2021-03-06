﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameCamera : MonoBehaviourSingleton<GameCamera> {

    [Header("Status")]
    [ReadOnly]  public LevelInstance levelInstance = null;
    [ReadOnly]  public GameBall ball = null;
                private Vector3 m_PreviousBallPosition;
    [ReadOnly]  public Vector3 desiredPosition;

    [Header("Input")]
    [ReadOnly]  public bool isAllowingInput = false;
    public float followSpeed = 0.02f;
    public float panSpeed = 30f;
    public float zoomSpeedTouch = 0.1f;
    public float zoomSpeedMouse = 15f;
    private Vector3 defaultOffset = new Vector3(0, 5, -25);
    public float camShake = 0;
    
    [Header("Restrictions")]
    public Transform leftBound;
    public Transform rightBound;
    public Transform topBound;
    public Transform botBound;
    public float maxZ = -9f;
    public float minZ = -45f;

    //Panning
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    private float m_ZoomAmount = 0;
    private float m_ZoomShakeAmount = 0;

    private void OnValidate() {
        if(maxZ > 0) { maxZ = 0; }
        if(maxZ < minZ) { maxZ = minZ; }
        if(minZ > maxZ) { minZ = maxZ; }
    }

    //Note: set on start because ball is set in levelinstance on awake.
    private void Start() {
        levelInstance = LevelInstance.Instance;
        desiredPosition = transform.position;

        if(levelInstance == null) {
            Debug.LogError("GameCamera could not find levelinstance!");
            return;
        }

        if(levelInstance.GetBall() == null) { 
            Debug.LogWarning("GameCamera could not find ball!");
            return;
        }
        
        ball = levelInstance.GetBall(); 
        m_PreviousBallPosition = ball.transform.position;
        
    }

    private void FixedUpdate() {
        ball = levelInstance.GetBall();
        if (ball == null) { return; }

        //In intro or when we are shooting, we need to move the camera.
        if (!ball.isSleeping || levelInstance.levelState == LevelState.INTRO) {
            Vector3 ballPosition = ball.transform.position;
            Vector3 ballDelta = (ballPosition - m_PreviousBallPosition) * 10;
            desiredPosition = ball.transform.position + (ballDelta * 2) + defaultOffset + new Vector3(0, 0, -ballDelta.magnitude);
        }

        //Apply position!
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, leftBound.transform.position.x, rightBound.transform.position.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, botBound.transform.position.y, topBound.transform.position.y);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);

        m_PreviousBallPosition = ball.transform.position;
        Vector3 shake = new Vector3(
            Random.Range(-camShake, camShake), 
            Random.Range(-camShake, camShake), 
            Random.Range(-camShake, camShake) + (m_ZoomAmount + Random.Range(-m_ZoomShakeAmount, m_ZoomShakeAmount)));
        transform.position = Vector3.Lerp(transform.position, desiredPosition, ball.completeSleep ? 0.2f : followSpeed) + shake;
        this.camShake *= 0.9f;

        m_ZoomAmount *= 0.9f;
        m_ZoomShakeAmount *= 0.9f;

        if (Input.GetMouseButtonUp(0)) {
            isAllowingInput = false;
        }
    }

    private void Update() {
        ball = levelInstance.GetBall();
        if (ball == null) { return; }

        if (!(!ball.isSleeping || levelInstance.levelState == LevelState.INTRO)) {
            if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer) {
                HandleTouch();
            } else {
                HandleMouse();
            }

        }
    }

    #region Panning and Zooming
    private void HandleMouse() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0) {
            ZoomCamera(scroll, zoomSpeedMouse);
        }

        if(Input.GetMouseButtonDown(0) && !IsPointerOverUIObject(-1)) {
            lastPanPosition = Input.mousePosition;
            isAllowingInput = true;
        }
        if(Input.GetMouseButton(0) && isAllowingInput) {
            PanCamera(Input.mousePosition);
        }
    }

    private void HandleTouch() {
        switch(Input.touchCount) {
            case 1:
                //panning
                wasZoomingLastFrame = false;
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began && !IsPointerOverUIObject(touch.fingerId)) {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                    isAllowingInput = true;
                }
                if(touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved && isAllowingInput) {
                    PanCamera(touch.position);
                }
                if(touch.fingerId == panFingerId && touch.phase == TouchPhase.Ended) {
                    isAllowingInput = false;
                }
                break;
            case 2: 
                //zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if(!wasZoomingLastFrame) {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                } else {
                    float newDist = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDist = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDist - oldDist;

                    ZoomCamera(offset, zoomSpeedTouch);
                    lastZoomPositions = newPositions;
                }

                break;
            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    private bool IsPointerOverUIObject(int id) {
        return EventSystem.current.IsPointerOverGameObject(id);
    }

    private void ZoomCamera(float offset, float speed) {
        desiredPosition.z += offset * speed;
    }

    private void PanCamera(Vector3 newPanPosition) {
        Vector3 offset = GetComponent<Camera>().ScreenToViewportPoint(lastPanPosition - newPanPosition);

        if (offset.magnitude <= 0.125f) {
            desiredPosition.x += offset.x * panSpeed;
            desiredPosition.y += offset.y * panSpeed;
        }

        lastPanPosition = newPanPosition;
    }
    #endregion

    #region Zoom shake
    public void SetZoomShake(float zoomAmount, float shakeEffect) {
        m_ZoomAmount = zoomAmount;
        m_ZoomShakeAmount = shakeEffect;
    }
    #endregion
}
