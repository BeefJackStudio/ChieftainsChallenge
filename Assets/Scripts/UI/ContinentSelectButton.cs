﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Button3D))]
public class ContinentSelectButton : MonoBehaviour {

    public GameLevelCollection levelSet;
    public GameLevelCollection requiredToUnlock;
    public GameObject lockImage;
    public SpriteRenderer continentImage;

    public SoundEffectCollection unlockedSounds;
    public SoundEffectCollection lockedSounds;

    private Button3D m_Button3D;
    private ContinentSelectMenu m_LevelMenu;
    private bool m_IsLocked = false;

    private void Awake() {
        m_Button3D = GetComponent<Button3D>();
        m_LevelMenu = GetComponentInParent<ContinentSelectMenu>();
    }

    private void Start() {
        m_IsLocked = false;
        if (requiredToUnlock != null) {
            m_IsLocked = !requiredToUnlock.IsCompleted();
        }

        lockImage.SetActive(m_IsLocked);
        continentImage.color = m_IsLocked ? Color.gray : Color.white;

        if (!m_IsLocked) {
            m_Button3D.onButtonClick.AddListener(() => { m_LevelMenu.OnContinentSelect(this, m_IsLocked); });
        }

        m_Button3D.onInteractSound = m_IsLocked ? lockedSounds : unlockedSounds;
    }

}
