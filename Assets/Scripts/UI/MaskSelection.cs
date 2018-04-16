using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaskSelection : MonoBehaviour {

    Collider2D c2d;
    public GameObject MaskPanel;
    public Text Title;
    public Text Description;

    public string MaskName;
    public string MaskDescription;

    private bool panelActive = false;

    private bool touchedDown = false;

    // Use this for initialization
    void Start() {
        c2d = GetComponent<Collider2D>();
    }

    void FixedUpdate() {
        if (!MaskPanel.activeSelf) {
            if (Input.touchCount > 0) {
                Vector3 wp;
                Vector2 touchPos;
                switch (Input.GetTouch(0).phase) {
                    case TouchPhase.Began:
                        wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        touchPos = new Vector2(wp.x, wp.y);
                        if (c2d == Physics2D.OverlapPoint(touchPos)) {
                            touchedDown = true;
                        }
                        break;
                    case TouchPhase.Ended:
                        wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        touchPos = new Vector2(wp.x, wp.y);
                        if (c2d == Physics2D.OverlapPoint(touchPos) && touchedDown) {

                            if (!panelActive && !MaskPanel.activeSelf) {
                                MaskPanel.SetActive(true);
                                Title.text = MaskName;
                                Description.text = MaskDescription;
                            }
                        }
                        touchedDown = false;
                        break;
                }
            }
        }
    }
}
