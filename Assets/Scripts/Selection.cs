using UnityEngine;
using System.Collections;

public class Selection : WorldObject 
{
	public GameObject visuals;
	public delegate void SelectionEventDelegate (Selection sender, Vector2int pos);
	public event SelectionEventDelegate OnMouseClick;
	
	Vector2int mousePosition;
	bool valid;
	
	void Start()
	{
		//Screen.showCursor = false;
	}
	
	void Update () 
	{
		MoveSelection();
		
		if (valid && Input.GetMouseButtonUp(0))
		{		
			if(OnMouseClick != null)
			{
				OnMouseClick(this, mousePosition);
			}
		}   
	}
	
	void MoveSelection()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
		{
			Vector3 p = hit.point;
			int x = Mathf.RoundToInt(p.x);
			int y = Mathf.RoundToInt(p.y);
			
			
			if(x < 0 || x >= Level.WorldSize || y < 0 || y >= Level.WorldSize)
			{
				//Out of bounds
				visuals.SetActive(false);
				valid = false;
				
				mousePosition = Vector2int.zero;
			}
			else
			{
				//Set cursor
				visuals.SetActive(true);			
				valid = true;
				
				mousePosition = new Vector2int(x, y);
				transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
			}	
		}
	}
}
