using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviourSingleton<TrajectoryRenderer> {

	void Update () {
        Vector2 angle = LevelInstance.Instance.shootAngle;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, angle));
    }
}
