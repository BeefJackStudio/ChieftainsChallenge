#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Button2D))]
public class Button2DEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		Button2D b = (Button2D) target;
	}

}
#endif
