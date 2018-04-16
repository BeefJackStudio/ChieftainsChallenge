using UnityEngine;
using System.Collections;

public class PowerMaskBall : PowerMask {
    public GameObject newBall;
    private GameObject ballObj;

    Transform oldParent;
    GameObject newBallInstance;

    void OnEnable() {
        NotificationsManager.OnBallEnabled += NotificationsManager_OnBallEnabled;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnBallEnabled -= NotificationsManager_OnBallEnabled;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnBallEnabled(Transform thisBall) {
        ballObj = thisBall.gameObject;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public override void ActivateIcon() {
        if (newBall != null) {
            GameObject obj = ballObj;
            oldParent = obj.transform.parent;
            newBallInstance = GameObject.Instantiate(newBall, obj.transform.position, obj.transform.rotation) as GameObject;
            newBallInstance.GetComponent<Rigidbody2D>().velocity = obj.GetComponent<Rigidbody2D>().velocity;
            newBallInstance.GetComponent<Rigidbody2D>().angularVelocity = obj.GetComponent<Rigidbody2D>().angularVelocity;
            newBallInstance.GetComponent<Ball>().parent = ballObj.GetComponent<Ball>();
            ballObj = obj; // needed to remember old 'ball' object
            obj.transform.parent = newBallInstance.transform;
            obj.SetActive(false);
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public override void DeactivateMask() {
        if (newBall != null && newBallInstance) {
            GameObject obj = ballObj;
            obj.GetComponent<Rigidbody2D>().velocity = newBallInstance.GetComponent<Rigidbody2D>().velocity;
            obj.GetComponent<Rigidbody2D>().angularVelocity = newBallInstance.GetComponent<Rigidbody2D>().angularVelocity;
            obj.transform.parent = oldParent;
            obj.SetActive(true);
            Destroy(newBallInstance);
            newBallInstance = null;
        }
        maskManager.powerBar.sprite = null;

    }
}
