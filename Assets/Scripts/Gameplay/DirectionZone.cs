using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum DirectionZoneDirection {
	RIGHT,
	LEFT
}

[System.Serializable]
public struct Area {
	public float top;
	public float bot;
	public float left;
	public float right;
	public float squaredArea;

	public Area(float _top, float _bot, float _left, float _right) {
		top = _top;
		bot = _bot;
		left = _left;
		right = _right;

		squaredArea = (top - bot) * (right - left);
	}
}

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionZone : MonoBehaviour {

	public static string DIRECTION_RIGHT_COLOUR = "02FF0032";
	public static string DIRECTION_LEFT_COLOUR = "0002FF32";

	private SpriteRenderer m_SpriteRenderer = null;

	[ReadOnly] public Area boundaries;
	public DirectionZoneDirection shootingDirection = DirectionZoneDirection.RIGHT;
	public Sprite onePixelWhite = null;

	private void Awake () {
		OnValidate();

		Color tempCol = m_SpriteRenderer.color;
		tempCol.a = 0;
		m_SpriteRenderer.color = tempCol;
	}
	
	private void Reset(){
		OnValidate();
	}

	private void OnValidate() {
		if(m_SpriteRenderer == null) { m_SpriteRenderer = GetComponent<SpriteRenderer>(); }
		m_SpriteRenderer.hideFlags = HideFlags.HideInInspector;

		if(onePixelWhite == null) { return; }
		m_SpriteRenderer.sprite = onePixelWhite;

		switch(shootingDirection) {
			case DirectionZoneDirection.RIGHT:
				GetComponent<SpriteRenderer>().color = ColorUtilities.hexToColor(DIRECTION_RIGHT_COLOUR);
				break;
			case DirectionZoneDirection.LEFT:
				GetComponent<SpriteRenderer>().color = ColorUtilities.hexToColor(DIRECTION_LEFT_COLOUR);
				break;
			default:
				m_SpriteRenderer.color = Color.white;
		    	break;
		}

		boundaries = new Area(	transform.position.y + m_SpriteRenderer.bounds.extents.y,
								transform.position.y - m_SpriteRenderer.bounds.extents.y,
								transform.position.x - m_SpriteRenderer.bounds.extents.x,
								transform.position.x + m_SpriteRenderer.bounds.extents.x
							);
	}

	public Vector2 GetAreaSize() {
		Vector2 result = new Vector2(m_SpriteRenderer.bounds.size.x, m_SpriteRenderer.bounds.size.y);
		return result;
	}

	public bool isPositionInZone(Vector3 objectPosition) {
		if(m_SpriteRenderer == null) { return false; } 

		if(objectPosition.x < boundaries.left || objectPosition.x > boundaries.right) {
			return false;
		}

		if(objectPosition.x < boundaries.bot || objectPosition.x > boundaries.top) {
			return false;
		}

		return true;
	}

}
