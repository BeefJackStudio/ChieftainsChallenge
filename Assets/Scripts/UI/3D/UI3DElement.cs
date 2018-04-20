using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UI3DElement : MonoBehaviour {
    public virtual void OnInteract(RaycastHit hit) { }
}
