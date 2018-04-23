using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    [ReadOnly] public LevelInstance levelInstance = null;

	void Awake () {
		levelInstance = GameObject.Find("LevelInstance").GetComponent<LevelInstance>();
	}

    void OnTriggerEnter2D(Collider2D coll) {
        if(levelInstance != null && (coll.gameObject.GetComponent("GameBall") as GameBall) != null) {
            levelInstance.EndGame();
        }
    }
}
