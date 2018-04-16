using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pages : MonoBehaviour {

    public List<RectTransform> TabTransforms;
    private int prevPage = 0;
    // Use this for initialization
    void Start() {
        Masks.PageChange += Masks_PageChange;
    }

    private void Masks_PageChange(int page) {

        TabTransforms[prevPage].sizeDelta = new Vector2(8.75f, 12);
        TabTransforms[page].sizeDelta = new Vector2(8.75f, 20);
        prevPage = page;

    }
}
