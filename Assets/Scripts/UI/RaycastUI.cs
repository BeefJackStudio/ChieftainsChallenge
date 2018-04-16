using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class RaycastUI : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        EventSystem system = EventSystem.current;
        List<RaycastResult> hits = new List<RaycastResult>();
        PointerEventData pointer = new PointerEventData(system);

        pointer.position = Input.mousePosition;
        system.RaycastAll(pointer, hits);

        for (int index = 0; index < hits.Count; index++) {
            Debug.Log("Hit: " + hits[index].gameObject.name);
        }
    }
}
