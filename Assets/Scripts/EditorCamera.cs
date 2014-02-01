using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorCamera : MonoBehaviour 
{
	#if UNITY_EDITOR
	public void Update () 
	{
		//Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
	}

	public void OnGUI()
	{
		//Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
	}
	#endif	
}
