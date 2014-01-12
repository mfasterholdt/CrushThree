using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldObject), true)]
public class WorldObjectEditor : Editor 
{

	public void OnSceneGUI()
	{
		Event e = Event.current;
		
		if(e.type == EventType.MouseUp || e.type == EventType.DragExited)
		{
			WorldObject obj = this.target as WorldObject;
			SnapObject(obj.transform);
		}
	}
	
	private void SnapObject(Transform t)
	{
		//Snap this object
		Vector3 pos = t.localPosition;
		
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		pos.z = 0;		
		
		t.localPosition = pos;
		
		//Snap parent
		if(t.parent != null)
		{
			SnapObject(t.parent);
		}
	}
}
