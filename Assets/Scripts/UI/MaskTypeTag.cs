using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskTypeTag : MonoBehaviour {

    public PowerMask Type;

    void Start() {
        if (Type != null) {
            GetComponent<Image>().sprite = Type.mask;
        }
    }
}
