using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuChest : MonoBehaviour {

    private Animator m_Animator;

    private void Awake() {
        m_Animator = GetComponent<Animator>();
    }

    public void Reset() {
        m_Animator.Play(0);
        m_Animator.playbackTime = 0;
        m_Animator.speed = 0;
    }

    public void PlayAnimation() {
        m_Animator.Play(0);
        m_Animator.playbackTime = 0;
        m_Animator.speed = 1;
    }

    public void OnChestAnimationOpen() {
        GetComponentInParent<ShopMenu>().ShowItems();
    }

}
