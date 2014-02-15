using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ArrowCap : Editor 
{
	static ArrowCap()
	{
		SceneView.onSceneGUIDelegate +=DrawArrows;
	}

	static void DrawArrows (SceneView view)
	{
		/*float arrowSize = 2f;

		Transform obj = Selection.activeTransform;

		if(!obj)
			return;

		Handles.color = Color.blue;
		Handles.ArrowCap(0, Vector3.right * 5f, obj.rotation, arrowSize);

		Vector3 currentScale = obj.localScale;

		Handles.ArrowCap(0, obj.position + Vector3.right, obj.rotation, arrowSize);

		if(GUI.changed)
			EditorUtility.SetDirty (obj);*/
	}
}

//Handles.ArrowCap(0, target.transform.position + new  Vector3(0,0,5), target.transform.rotation, arrowSize);