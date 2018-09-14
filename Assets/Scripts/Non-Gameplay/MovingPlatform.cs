using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	private Vector2 startPosition; 

	private Vector2 newPosition; 

	[SerializeField] private int speed = 3;
	[SerializeField] private float maxDistance = 1;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		newPosition = transform.position;

	}

	// Update is called once per frame
	void Update () {
		newPosition.y = startPosition.y + (maxDistance * Mathf.Sin(Time.time * speed));
		transform.position = newPosition;
	}


	private void OnCollisionEnter2D(Collision2D col)
	{
			if (col.gameObject.name == "ball") {
			Debug.Log ("Detected"); 
			//collision.collider.transform.SetParent (transform);
		}
	}

//	private void OnCollisionExit2D(Collision2D collision)
//	{
//			if (collision.gameObject.tag == "Player" ()) {
//			
//			//collision.collider.transform.SetParent (null); 
//		}
//	}
//
//
}
	
