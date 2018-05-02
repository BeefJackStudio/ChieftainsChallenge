using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLerper : MonoBehaviour {

	[ReadOnly]		public RectTransform rectTransform;
	[ReadOnly]		public Vector3 beginPosition;
					public Vector3 targetPosition;
	[Range(0,1)]	public float speed = 0.15f;


	// Use this for initialization
	void Awake () {
		rectTransform = (RectTransform) this.transform;

		beginPosition = rectTransform.anchoredPosition;
		targetPosition = rectTransform.anchoredPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 0.1) {
			rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, speed);
		}
	}
}
