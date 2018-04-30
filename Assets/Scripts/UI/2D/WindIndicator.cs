using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindIndicator : MonoBehaviour {

    [Header("Status")]
    [ReadOnly] public LevelInstance levelInstance;
    [ReadOnly]	public float pinAngle = 0f;

	[Header("Pin Setup")]
	public GameObject pin = null;
	public float wobbleSpeed = 4f;
	public float wobbleRange = 4f;

    [Header("Text")]
    public TextMeshProUGUI text;

    private float velocityTargetNumber = 0;
    private float velocityDisplayNumber = 0;

    private void Start() {
        levelInstance = LevelInstance.Instance;
        if (levelInstance == null) { return; }

        if (!levelInstance.enableWind) {
            gameObject.SetActive(false);
            return;
        } else {
            if (pin == null) {
                Debug.LogWarning("WindIndicator: You didn't setup a pin in config!");
                return;
            }
        }

        levelInstance.OnNextTurn += UpdateWind;
    }
	
	void Update () {

        //apply pin rocking effect
        float f = Mathf.Sin(Time.time * wobbleSpeed) * wobbleRange;
		pin.transform.localRotation = Quaternion.Lerp(pin.transform.localRotation, Quaternion.AngleAxis(f + pinAngle, Vector3.forward), 0.1f);

        velocityDisplayNumber = Mathf.Lerp(velocityDisplayNumber, velocityTargetNumber, 0.1f);

        string velText = velocityDisplayNumber.ToString().Length >= 3 ? velocityDisplayNumber.ToString().Substring(0, 3) : "" + velocityDisplayNumber;
        text.text = velText;
	}

    public void UpdateWind() {
        pinAngle = -(Mathf.Atan2(levelInstance.windForce.x, levelInstance.windForce.y) * Mathf.Rad2Deg);
        velocityTargetNumber = levelInstance.windForce.magnitude;
    }
}
