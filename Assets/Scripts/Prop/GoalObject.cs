//
//  GoalObject.cs
//  
//  Created by Scott Mitchell on (DATE).
//  Copyright (c) 2015 Scott Mitchell. All rights reserved.
//

using UnityEngine;
using System.Collections;

public class GoalObject : MonoBehaviour {

    private float timer;
    private float delayTime = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.name == "Ball") {
            StartCoroutine(StartEndGame(collision));
        }
    }

    IEnumerator StartEndGame(Collision2D collision) {
        timer += Time.deltaTime;

        if (timer < delayTime)
            yield return null;

        GameManager.Instance.gameState = GameManager.GameState.GameFinished;
        collision.collider.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}