using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Button2D : Button {

    public Vector3 ClickScale = new Vector3(0.9f, 0.9f, 0.9f);
    public Vector3 DefaultScale = new Vector3(1f, 1f, 1f);

    [Header("Music")]
	[ReadOnly]	public SoundEffectsPlayer sepRef = null;
                public SoundEffectCollection onClickSound;

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if(sepRef == null) { sepRef = SoundEffectsPlayer.Instance; }
        transition = Transition.None;   //yuck.
        base.OnPointerDown(eventData);

        RectTransform transform = (RectTransform)gameObject.GetComponent("RectTransform");
        if (transform != null) {
            transform.localScale = ClickScale;
        }
        if (sepRef != null) {
            SoundEffectsPlayer.Instance.PlaySFX(onClickSound);
        }
    }

    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        RectTransform transform = (RectTransform)gameObject.GetComponent("RectTransform");
        if (transform != null) {
            transform.localScale = DefaultScale;
        }
    }
}
