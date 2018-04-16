using UnityEngine;
using System.Collections;

public class FollowScreenCoordinates : MonoBehaviour {

    public GameObject followObject;
    public Camera gameCamera;

    private Vector2 screenCoordinates;

    void Update() {
        screenCoordinates = gameCamera.WorldToScreenPoint(followObject.transform.position);

        transform.position = screenCoordinates;
    }
}
