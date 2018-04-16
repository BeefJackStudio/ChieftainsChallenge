using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PowerMask : MonoBehaviour {
    public MaskManager maskManager;

    public AnimationCurve powerCurve;
    public Sprite mask;
    public Sprite outline;
    public Sprite[] zone;
    public Sprite[] zoneActive;
    public Sprite icon;

    GradientTexture gradTex;

    // Use this for initialization
    void Start() {

    }

    public void Init(int index) {
        if (gradTex == null) {
            gradTex = new GradientTexture();
        }
        gradTex.Create(powerCurve, 512, new Color(0, 1, 0), new Color(1, 1, 0), new Color(1, 0, 0));
        if (index != -1) {
            maskManager.UIMaskes[index].GetComponent<Image>().sprite = mask;
        }
    }

    public virtual void Use() {
        if (zone[0] != null) {
            float width = zone[0].textureRect.width;
            float height = zone[0].textureRect.height;
            if (width > height) {
                float scale = 200.0f / width;
                maskManager.powerBar.rectTransform.sizeDelta = new Vector2(200, height * scale);

            } else if (height > width) {
                float scale = 200.0f / height;
                maskManager.powerBar.rectTransform.sizeDelta = new Vector2(width * scale, 200);

            } else {
                maskManager.powerBar.rectTransform.sizeDelta = new Vector2(200, 200);
            }
        }

        maskManager.powerBar.sprite = zone[0];
        maskManager.playerMask.sprite = mask;
        maskManager.playerMaskOutline.sprite = outline;
    }

    public virtual void UseActive() {
        maskManager.powerBar.sprite = zoneActive[0];
    }

    // Update is called once per frame
    void Update() {

    }

    public virtual bool TestPoint(Vector2 pos) {
        EventSystem system = EventSystem.current;
        List<RaycastResult> hits = new List<RaycastResult>();
        PointerEventData pointer = new PointerEventData(system);

        pointer.position = pos;
        system.RaycastAll(pointer, hits);

        for (int index = 0; index < hits.Count; index++) {
            if (hits[index].gameObject == maskManager.powerBar.gameObject) {
                return true;
            }
        }
        return false;
    }

    public virtual void ActivateIcon() {

    }

    public virtual void DeactivateMask() {

    }
}
