using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldObject), true)]
[CanEditMultipleObjects]
public class TileEditor : WorldObjectEditor 
{
	/*void OnSceneGUI () 
	{
		base.OnSceneGUI();

		Tile t = target as Tile;

		if(!t)
			return;

		Vector3 scale = t.transform.localScale;

		float test =  Handles.ScaleValueHandle(scale.x, t.transform.position, Quaternion.identity, 30f, Handles.ArrowCap, 1f);      							

		Debug.Log (test);

		//if (GUI.changed)
			EditorUtility.SetDirty (target);
	}*/
}
