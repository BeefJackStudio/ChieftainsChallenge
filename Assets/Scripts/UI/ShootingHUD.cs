using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingHUD : MonoBehaviourSingleton<ShootingHUD> {

    public RectTransform powerBar;
    public AnimationCurve curve;

    [Header("Move Animation (hiding)")]
    public float maDistance = 150;       
    public List<ObjectLerper> maDownMoveTransforms;
    public List<ObjectLerper> maRightMoveTransforms;

    private List<ObjectLerper> m_ObjectsToLerp = new List<ObjectLerper>();
    private Dictionary<ObjectLerper, Vector2> m_OriginalPositions = new Dictionary<ObjectLerper, Vector2>();

    private float m_RotationSpeed = 36;
    private bool m_IsAimingLeft = false;
    private bool m_IsAimingRight = false;
    private bool m_IsShooting = false;
    private float m_ShootStartTime;

    private void Awake() {
        m_ObjectsToLerp.AddRange(maDownMoveTransforms);
        m_ObjectsToLerp.AddRange(maRightMoveTransforms);

        foreach(ObjectLerper o in m_ObjectsToLerp) {
            m_OriginalPositions.Add(o, o.GetComponent<RectTransform>().anchoredPosition);
        }
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

        if (m_IsShooting) {
            OnPowerChange(curve.Evaluate((Time.time - m_ShootStartTime) * 0.5f));
        }
    }

    public void OnShootDown() {
        m_IsShooting = true;
        m_ShootStartTime = Time.time;
    }

    public void OnShootUp() {
        LevelInstance.Instance.ShootBall();
        m_IsShooting = false;
    }

    public void OnPowerChange(float power) {
        LevelInstance.Instance.normalizedShootPower = power + 0.001f;
        powerBar.localScale = new Vector3(powerBar.localScale.x, power, powerBar.localScale.z);
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
        foreach(ObjectLerper o in m_ObjectsToLerp) {
            foreach(Button button in o.GetComponentsInChildren<Button>()) {
                button.interactable = false;
            }

            Vector2 temp = m_OriginalPositions[o];

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
        OnPowerChange(0);

        foreach (ObjectLerper o in m_ObjectsToLerp) {
            foreach(Button button in o.GetComponentsInChildren<Button>()) {
                button.interactable = true;
            }
            Vector2 temp = m_OriginalPositions[o];

            if(!bForce) {
                o.targetPosition = temp;
            } else {
                o.rectTransform.anchoredPosition = temp;
                o.targetPosition = temp;
            }
        }
    }
}
