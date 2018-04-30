using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingHUD : MonoBehaviour {

    private float m_RotationSpeed = 36;
    private bool m_IsAimingLeft = false;
    private bool m_IsAimingRight = false;

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
}
