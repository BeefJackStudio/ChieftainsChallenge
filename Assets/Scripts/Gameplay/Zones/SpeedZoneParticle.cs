using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpeedZoneParticle : MonoBehaviour {

	[ReadOnly] public ParticleSystem particleSystem;
	public float multiplier = 2;

	private void Awake() {
		particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	private void LateUpdate () {
		if(LevelInstance.Instance == null || LevelInstance.Instance.GetBall() == null) { return; }

		var velocityOverLifetime = particleSystem.velocityOverLifetime;
		var main = particleSystem.main;
		Vector2 temp = Vector2.zero;
		float angle = 1;

		if(LevelInstance.Instance.GetBall().isSleeping) {
			temp = LevelInstance.Instance.shootAngle.normalized;
		} else {
			temp = LevelInstance.Instance.GetBall().GetComponent<Rigidbody2D>().velocity.normalized;
		}
		

		velocityOverLifetime.x = -temp.y * multiplier;
		velocityOverLifetime.y = temp.x * multiplier;

		angle = Mathf.Atan2(-temp.y * multiplier, temp.x * multiplier) + (Mathf.Deg2Rad * 90);

		main.startRotationZ = angle;

	}
}