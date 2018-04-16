using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PowerMaskSunBall : PowerMaskBall {
    public GameObject FireEffect;
    RectTransform sun;

    Color col;
    bool dir = true;
    float duraction = 10;
    float startTime;

    Ball ball;

    Vector3 sunStartPos;
    Vector3 sunPos;

    void OnEnable() {
        NotificationsManager.OnBallEnabled += NotificationsManager_OnBallEnabled;

        sun = maskManager.subBar.GetComponent<RectTransform>();
        sun.GetComponent<Image>().sprite = zone[1];
        col = new Color(1, 1, 1, 0);
        startTime = Time.time;
        sunStartPos = sun.localPosition;
        sunPos = new Vector3(0, 0, sunStartPos.z);
    }

    void OnDisable() {
        NotificationsManager.OnBallEnabled -= NotificationsManager_OnBallEnabled;
    }

    private void NotificationsManager_OnBallEnabled(Transform thisBall) {
        ball = thisBall.GetComponent<Ball>();
    }

    void OnDestroy() {
        maskManager.powerBar.raycastTarget = true;
        if (sun != null) {
            sun.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    void Update() {
        col.a = Mathf.Lerp(col.a, 1, Time.deltaTime);
        sun.GetComponent<Image>().color = col;
        Vector3 pos = sun.localPosition;
        float t = (Time.time - startTime) / duraction;
        if (dir) {

            sunPos.x = Mathf.SmoothStep(sunPos.x, 180, t);
            if (sunPos.x.Aprox(180, 1)) {
                dir = false;
                startTime = Time.time;
            }

        } else {
            sunPos.x = Mathf.SmoothStep(sunPos.x, 0, t);
            if (sunPos.x.Aprox(0, 1)) {
                dir = true;
                startTime = Time.time;
            }
        }
        sunPos.y = (Mathf.Sin((sunPos.x) * Mathf.Deg2Rad) * 80);
        sun.localPosition = new Vector3(sunPos.x + sunStartPos.x, sunPos.y + sunStartPos.y, sunPos.z);
    }

    public override void Use() {
        base.Use();
        if (sun != null) {
            sun.GetComponent<Image>().sprite = zone[1];
        }
    }

    public override void UseActive() {
        maskManager.powerBar.sprite = zoneActive[0];
        sun.GetComponent<Image>().sprite = zoneActive[1];
    }

    public override bool TestPoint(Vector2 pos) {
        EventSystem system = EventSystem.current;
        List<RaycastResult> hits = new List<RaycastResult>();
        PointerEventData pointer = new PointerEventData(system);

        pointer.position = pos;

        system.RaycastAll(pointer, hits);

        Debug.Log("Num Hits:" + hits.Count);

        for (int index = 0; index < hits.Count; index++) {
            Debug.Log("Hit:" + hits[index].gameObject.name);
            if (hits[index].gameObject == sun.gameObject) {
                return true;
            }
        }
        return false;
    }

    public override void ActivateIcon() {
        ball.GetComponent<TrailRenderer>().enabled = false;
        GameObject fire = Instantiate(FireEffect) as GameObject;
        fire.transform.parent = ball.transform;
        fire.transform.localPosition = Vector3.zero;
        fire.transform.localRotation = Quaternion.identity;
        ball.UpdatePower(1);
        ball.UpdateForceAngle(0);
        ball.ShootBall();
    }

    public override void DeactivateMask() {
        maskManager.powerBar.sprite = null;
    }
}
