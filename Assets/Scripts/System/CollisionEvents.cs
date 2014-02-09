using UnityEngine;
using System.Collections;

public class CollisionEvents : MonoBehaviour 
{

	public delegate void TriggerEvent(Collider2D collider); 
	public TriggerEvent OnTriggerEnter;
	public TriggerEvent OnTriggerExit;
	
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(OnTriggerEnter != null)
			OnTriggerEnter(collider);
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if(OnTriggerExit != null)
			OnTriggerExit(collider);
	}
}
