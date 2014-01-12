using UnityEngine;
using System.Collections;

public class Player : Tile 
{	
	private float playerMoveSpeed = 5f;
	private Vector3 moveDir;
	
	public override void Initialize ()
	{
		base.Initialize ();
	}
	
	public override void FixedUpdate()
	{
		moveDir.x = Input.GetAxis("Horizontal");
		moveDir.y = Input.GetAxis("Vertical");
	}
}
