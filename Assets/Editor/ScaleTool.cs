using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ArrowCap : Editor 
{
	float arrowSize = 1;
	
	void OnSceneGUI () 
	{
		Debug.Log("hey");
		//Handles.color = Color.red;

		//Handles.ArrowCap(0, target.transform.position +  new Vector3(5,0,0), target.transform.rotation, arrowSize);


		//Handles.color = Color.green;

		//Handles.ArrowCap(0, target.transform.position + new Vector3(0,5,0), target.transform.rotation, arrowSize);    	

		Transform obj = Selection.activeTransform;

		if(!obj)
			return;

		Handles.color = Color.blue;
		Handles.ArrowCap(0, Vector3.right * 5f, obj.transform.rotation, arrowSize);
		//Handles.ArrowCap(0, target.transform.position + new  Vector3(0,0,5), target.transform.rotation, arrowSize);
	}
}