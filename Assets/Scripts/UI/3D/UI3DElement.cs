using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UI3DElement : MonoBehaviour {
    public SoundEffectCollection onInteractSound;

    public virtual void OnInteract(RaycastHit hit) {
        if (onInteractSound != null) SoundEffectsPlayer.Instance.PlaySFX(onInteractSound);
    }
}
