using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BouncyZone : MonoBehaviour {

	[ReadOnly] public PhysicsMaterial2D originalPhysMat;
	public PhysicsMaterial2D zoneMaterial;

	private void Awake() {
		this.GetComponent<BoxCollider2D>().isTrigger = true;
	}

	private void OnValidate() {
		this.GetComponent<BoxCollider2D>().hideFlags = HideFlags.HideInInspector;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.GetComponent<GameBall>() == null) return;
		originalPhysMat = other.gameObject.GetComponent<Rigidbody2D>().sharedMaterial;
		other.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = zoneMaterial;
		other.gameObject.GetComponent<CircleCollider2D>().sharedMaterial = zoneMaterial;
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.GetComponent<GameBall>() == null) return;
		other.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = originalPhysMat;
		other.gameObject.GetComponent<CircleCollider2D>().sharedMaterial = originalPhysMat;
	}

}
