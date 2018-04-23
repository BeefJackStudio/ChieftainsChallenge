using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D coll) {
        //Not a ball, cancel.
        if ((coll.gameObject.GetComponent("GameBall") as GameBall) == null) {
            return;
        }

        Debug.Log("GOAL");
    }
}
