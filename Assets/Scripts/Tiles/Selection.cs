using UnityEngine;
using System.Collections;

public class Selection : WorldObject 
{
	public GameObject visuals;
	public delegate void SelectionEventDelegate (Selection sender, Vector3 pos);
	public event SelectionEventDelegate OnMouseClick;
	public event SelectionEventDelegate OnMouseRelease;
	public LayerMask layerMask;

	public Vector3 mousePosition;

	private bool valid;

	[HideInInspector]
	public bool dragging;

	void Start()
	{
		//Screen.showCursor = false;
	}
	
	void Update () 
	{
		MoveSelection();
		
		if (valid && Input.GetMouseButtonDown(0))
		{		
			dragging = true;
			
			if(OnMouseClick != null)
				OnMouseClick(this, mousePosition);
		}   

		if(Input.GetMouseButtonUp(0))
		{
			dragging = false;

			if(OnMouseRelease != null)
				OnMouseRelease(this, mousePosition);			
		}
	}

	void MoveSelection()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layerMask))
		{
			Vector3 p = hit.point;
			int x = Mathf.RoundToInt(p.x);
			int y = Mathf.RoundToInt(p.y);
			
			if(x < 0 || x >= Level.WorldSize || y < 0 || y >= Level.WorldSize)
			{
				//Out of bounds
				visuals.SetActive(false);
				valid = false;
				
				mousePosition = Vector3.zero;
			}
			else
			{
				//Set cursor
				visuals.SetActive(true);			
				valid = true;

				mousePosition.x = x;
				mousePosition.y = y;
				transform.position = mousePosition;
			}	
		}
	}
}
