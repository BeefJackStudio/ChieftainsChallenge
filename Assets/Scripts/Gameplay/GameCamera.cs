using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

    [Header("Status")]
    [ReadOnly]  public LevelInstance levelInstance = null;
    [ReadOnly]  public GameBall ball = null;
                private Vector3 m_PreviousBallPosition;
    [ReadOnly]  public Vector3 desiredPosition;

    [Header("Config")]
    public Vector3 defaultOffset = new Vector3(0, 5, -25);
    public float followSpeed = 0.1f;

    //Note: set on start because ball is set in levelinstance on awake.
    private void Start() {
        levelInstance = GameObject.Find("LevelInstance").GetComponent<LevelInstance>();
        desiredPosition = transform.position;

        if(levelInstance == null) {
            Debug.LogError("GameCamera could not find levelinstance!");
            return;
        }

        if(levelInstance.GetBall() == null) { 
            Debug.LogError("GameCamera could not find ball!");
            return;
        }
        
        ball = levelInstance.GetBall(); 
        m_PreviousBallPosition = ball.transform.position;
        
    }

    private void FixedUpdate() {
        //Update ball in case it had changed.
        if(levelInstance.GetBall() != ball) {
            ball = levelInstance.GetBall();
            if(ball == null) { return; }
        }

        //In intro or when we are shooting, we need to move the camera.
        if(!ball.isSleeping || levelInstance.levelState == LevelState.intro) {
            Vector3 ballPosition = ball.transform.position;
            Vector3 ballDelta = (ballPosition - m_PreviousBallPosition) * 10;
            desiredPosition = ball.transform.position + (ballDelta * 2) + defaultOffset + new Vector3(0, 0, -ballDelta.magnitude);

            //if cam position is close to desired, and levelstate is now intro, set it to ingame.
            if(Vector3.Distance(transform.position, desiredPosition) <= 2 && levelInstance.levelState == LevelState.intro) {
                levelInstance.levelState = LevelState.inGame;
            }
        }

        //Apply position!
        m_PreviousBallPosition = ball.transform.position;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed);
    }
}
