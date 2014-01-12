using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorTools  
{
	[MenuItem("GameObject/Reset/LocalPosition %#r")]
	static void ResetTransform()
	{
		GameObject obj = UnityEditor.Selection.activeGameObject;
		
		if(obj != null)
		{
			Undo.RecordObject(obj, "Reset Local Position");
			obj.transform.localPosition = Vector3.zero;
		}
	}
	
	[MenuItem("GameObject/Reset/OrientationAndScale %#e")]
	static void ResetOrientationAndScale()
	{
		GameObject obj = UnityEditor.Selection.activeGameObject;
		
		if(obj != null)
		{
			Undo.RecordObject(obj, "Reset Local Rotation and Local Scale");
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
		}
	}
}
