using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindIndicator : MonoBehaviour {

    [Header("Status")]
    [ReadOnly] 	public LevelInstance levelInstance = null;
	[ReadOnly]	public float pinAngle = 0f;

	[Header("Pin Setup")]
	public GameObject pin = null;
	public float speedMultiplier = 4f;
	public float angleRange = 10f;


	void Start () {
		levelInstance = GameObject.Find("LevelInstance").GetComponent<LevelInstance>();
		if(levelInstance == null) { return; }

		if(!levelInstance.enableWind) {
			gameObject.SetActive(false);
			return;
		} else {
			if(pin == null) { 
				Debug.LogWarning("WindIndicator: You didn't setup a pin in config!");
				return; 
			}

			pinAngle = -(Mathf.Atan2(levelInstance.windDirection.x, levelInstance.windDirection.y) * Mathf.Rad2Deg);
			pin.transform.rotation = Quaternion.AngleAxis(pinAngle, Vector3.forward);
		}
	}
	
	void Update () {
		if(!gameObject.active) { return; }

		//apply pin rocking effect
		float f = Mathf.Sin(Time.time * speedMultiplier) * angleRange;
		pin.transform.rotation = Quaternion.AngleAxis(f + pinAngle, Vector3.forward);
	}
}
