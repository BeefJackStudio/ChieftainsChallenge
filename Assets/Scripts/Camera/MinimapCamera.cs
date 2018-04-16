using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;

public class MinimapCamera : MonoBehaviour {
    public Camera Minimap;

    public Vector3 targetPos;

    public float targetFOV;



    public Vector2 positionDiff;

    public Vector3 lastTouch;
    public Vector3 thisTouch;

    private Vector3 camAcceleration = Vector3.zero;
    private float drag = 0.95f;
    private float camSpeed = 0.05f;
    public Transform target;
    public float xEdgeOffset = 7;
    public float yEdgeOffset = 5;
    public float groundOffset = 9.5f;
    private float offset;

    float velocityDelay;

    public void OnPointerEnter(PointerEventData data) {
        Debug.Log("Enter!");
    }

    public void OnPointerExit(PointerEventData data) {
        Debug.Log("Exit!");
    }



    void Start() {
        Minimap = GetComponent<Camera>();

        targetPos = transform.position;
        targetFOV = GetComponent<Camera>().fieldOfView;

        InputManager.inputMoved += TouchInput;
        InputManager.inputStart += TouchStart;

        velocityDelay = 0;
    }
    void TouchStart() {
        lastTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
    }
    public void TouchInput() {
        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
    }


    public void OnDrag() {
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
            targetFOV += deltaMagnitudeDiff * 0.5f;
            targetFOV = Mathf.Clamp(targetFOV, 40, 100);

        }

        Minimap.fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFOV, Time.fixedDeltaTime);
        lastTouch.z = -transform.position.z;
        thisTouch.z = -transform.position.z;
        Vector3 p1 = Minimap.ScreenToWorldPoint(lastTouch);
        Vector3 p2 = Minimap.ScreenToWorldPoint(thisTouch);

        Vector3 inputForce = p1 - p2;

        transform.position += inputForce * camSpeed;

        positionDiff = Vector2.zero;
        velocityDelay = 2;
        lastTouch = thisTouch;
    }

}

