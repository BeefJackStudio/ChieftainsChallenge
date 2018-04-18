using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererSorter : MonoBehaviour {

    public int orderOffset = 0;

    [ContextMenu("Sort sprites")]
    private void Sort() {
        List<SpriteRenderer> spriteList = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        spriteList.Sort((s1, s2) => s1.transform.position.z.CompareTo(s2.transform.position.z));

        int sortOrder = orderOffset;
        foreach(SpriteRenderer s in spriteList) {
            s.sortingOrder = sortOrder;
            sortOrder--;
        }
    }
}
