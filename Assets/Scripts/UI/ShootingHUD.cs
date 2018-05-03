using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingHUD : MonoBehaviourSingleton<ShootingHUD> {

    private float m_RotationSpeed = 36;
    private bool m_IsAimingLeft = false;
    private bool m_IsAimingRight = false;

    [Header("Move Animation (hiding)")]
    public float maDistance = 150;       
    public List<ObjectLerper> maDownMoveTransforms;
    public List<ObjectLerper> maRightMoveTransforms;

    private List<ObjectLerper> objectsToLerp;

    private void Awake() {
        objectsToLerp = new List<ObjectLerper>();
        objectsToLerp.AddRange(maDownMoveTransforms);
        objectsToLerp.AddRange(maRightMoveTransforms);
    }

    private void Start() {
        Hide(true);
    }

    private void Update() {
        if (m_IsAimingLeft) {
            LevelInstance.Instance.shootAngle = LevelInstance.Instance.shootAngle.Rotate(m_RotationSpeed * Time.deltaTime);
        }

        if (m_IsAimingRight) {
            LevelInstance.Instance.shootAngle = LevelInstance.Instance.shootAngle.Rotate(-m_RotationSpeed * Time.deltaTime);
        }
    }

    public void OnShoot() {
        LevelInstance.Instance.ShootBall();
    }

    public void OnPowerChange(Slider slider) {
        LevelInstance.Instance.normalizedShootPower = slider.normalizedValue;
    }

    public void OnAimLeftDown() {
        m_IsAimingLeft = true;
    }

    public void OnAimLeftUp() {
        m_IsAimingLeft = false;
    }

    public void OnAimRightDown() {
        m_IsAimingRight = true;
    }

    public void OnAimRightUp() {
        m_IsAimingRight = false;
    }

    public void Hide(bool bForce = false) {
        foreach(ObjectLerper o in objectsToLerp) {
            foreach(Button button in o.GetComponentsInChildren<Button>()) {
                button.interactable = false;
            }
            Vector2 temp = o.rectTransform.anchoredPosition;

            if(maRightMoveTransforms.Contains(o)) {
                temp.x += maDistance;
            }

            if(maDownMoveTransforms.Contains(o)) {
                temp.y -= maDistance;
            }

            if(!bForce) {
                o.targetPosition = temp;
            } else {
                o.rectTransform.anchoredPosition = temp;
                o.targetPosition = temp;
            }
        }
    }

    public void Show(bool bForce = false) {
        foreach(ObjectLerper o in objectsToLerp) {
            foreach(Button button in o.GetComponentsInChildren<Button>()) {
                button.interactable = true;
            }
            Vector2 temp = o.rectTransform.anchoredPosition;

            if(maRightMoveTransforms.Contains(o)) {
                temp.x -= maDistance;
            }

            if(maDownMoveTransforms.Contains(o)) {
                temp.y += maDistance;
            }

            if(!bForce) {
                o.targetPosition = temp;
            } else {
                o.rectTransform.anchoredPosition = temp;
                o.targetPosition = temp;
            }
        }
    }
}
