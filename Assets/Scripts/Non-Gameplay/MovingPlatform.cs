using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	[SerializeField] private int speed = 3;
	[SerializeField] private int delayTime = 2;
	[SerializeField] private int direction = 1;
	private int directionhold;


	// Use this for initialization
	void Start () {
		StartCoroutine (ChangeDirection ());
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (0, direction * speed * Time.deltaTime, 0);
	}


	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == null) {
			
		} 

		if (col.gameObject.tag == "ElevatorStop") {
			StartCoroutine (ChangeDirection ());
		}
	
		}
		

	IEnumerator ChangeDirection(){
		Debug.Log ("Hit");
		directionhold = direction; 
		direction = 0; 


		yield return new WaitForSeconds (delayTime);

		if (directionhold == -1) {
			direction = 1 ;
		}

		if (directionhold == 1) {
			direction = -1 ;
		}

		//yield return new WaitForSeconds (2);

	}
	
	}
	 

	
