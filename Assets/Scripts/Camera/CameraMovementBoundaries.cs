using UnityEngine;
using System.Collections;

public class CameraMovementBoundaries : MonoBehaviour {

    public Transform gameCamera;
    public BallCamera ballCam;
    public Vector2 topLeftCorner;
    public Vector2 bottomRightCorner;

    void Update() {
        if (ballCam.State == BallCamera.CameraStat.TrackBall)
            return;

        if (gameCamera.position.x < topLeftCorner.x) {
            gameCamera.position = new Vector3(topLeftCorner.x, gameCamera.position.y, gameCamera.position.z);
        } else if (gameCamera.position.x > bottomRightCorner.x) {
            gameCamera.position = new Vector3(bottomRightCorner.x, gameCamera.position.y, gameCamera.position.z);
        }

        if (gameCamera.position.y > topLeftCorner.y) {
            gameCamera.position = new Vector3(gameCamera.position.x, topLeftCorner.y, gameCamera.position.z);
        } else if (gameCamera.position.y < bottomRightCorner.y) {
            gameCamera.position = new Vector3(gameCamera.position.x, bottomRightCorner.y, gameCamera.position.z);
        }
    }
}
