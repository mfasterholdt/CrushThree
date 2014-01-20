using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{	
	public Transform visuals;
	public float playerMoveSpeed = 5f;
	public float climbSpeed = 5f;
	public float jumpForce = 50f;
	public float gravity = -15f;

	public Vector3 cameraOffset;
	private Camera cam;
	
	private Vector3 moveDir;

	private bool grounded = true;
	private float vel;

	private bool camFollow;
	private float cameraEaseIn;
	private float wallCollision;
	
	List<Trigger> triggers = new List<Trigger>();

	public LayerMask environmentMask;
	//****Create STATES to handle all the stuffs!!!

	public delegate void State();
	public State state;

	public TileCandy carrying;

	public float facing = 1;

	void Start()
	{
		cam = Camera.main;
		SetMoveState();
	}

	void SetMoveState()
	{
		state = MoveState;
	}

	void MoveState()
	{
		//Movement
		Vector3 nextPos = rigidbody2D.transform.position;
		float inputX = Input.GetAxis("Horizontal");
		
		if(wallCollision == 0 || Mathf.Sign(inputX) == wallCollision)		
		{
			nextPos.x += inputX * playerMoveSpeed * Time.deltaTime;
		}

		//Rotating
		if(inputX != 0)
		{
			facing = Mathf.Sign(inputX);
			visuals.localRotation = Quaternion.Euler(0, facing == 1 ? 0 : 180f, 0);
		}

		//Climbing
		float inputY = Input.GetAxis("Vertical");
		
		if(triggers.Count > 0)
		{
			if(Mathf.Abs(inputY) > 0.3f)
			{
				nextPos.y += inputY * climbSpeed * Time.deltaTime;
			}
		}
		else
		{
			//Gravity
			rigidbody2D.AddForce(Vector2.up * gravity);
		}
	
		
		//Apply Movement
		rigidbody2D.transform.position = nextPos;
	}
	
	void FixedUpdate()
	{		
		if(state != null)
			state();
	}

	void Update()
	{
		//Jump
		moveDir.x = Input.GetAxis("Horizontal");
		moveDir.y = Input.GetAxis("Vertical");
		
		if(grounded && Input.GetKey(KeyCode.Space))
		{
			rigidbody2D.AddForce(Vector2.up * jumpForce);
			grounded = false;
		}

		//Pickup
		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			AttemptPickup();
		}

		//Drop
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			AttemptDrop();
		}

		
		//Carry
		if(carrying)
		{
			carrying.transform.position = transform.position + transform.up * 1.1f;
		}

		//Camera Follow
		// *** make camera ease in on movement, (nextPos.x != rigidbody2D.transform.position.x)

		/*cameraEaseIn += Time.deltaTime * 0.3f;
		cameraEaseIn = Mathf.Min (cameraEaseIn, 1);

		Vector3 nextCamPos = cam.transform.position;*/
		
		Vector3 camTarget = transform.position - transform.forward * cameraOffset.x + transform.up * cameraOffset.y;
		//nextCamPos += ((camTarget - nextCamPos) * Time.deltaTime * 4.25f) * cameraEaseIn;*
		
		cam.transform.position = camTarget;
		
	}

	void AttemptPickup()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, visuals.right, 1f, environmentMask);

		if(hit)
		{
			TileCandy tile = hit.collider.gameObject.GetComponent<TileCandy>();

			if(tile)
			{
				carrying = tile;

				tile.SetCarryState();
			}
		}
	}

	void AttemptDrop()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, visuals.right, 1f, environmentMask);
		
		if(!hit)
		{
			Vector3 dropPos = transform.position + visuals.right;

			carrying.transform.position = dropPos;

			carrying.SetIdleState();

			carrying = null;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			ContactPoint2D c = col.contacts[i];
			
			float dot = Vector3.Dot(c.normal, Vector3.up); 
			
			if(dot > 0.6f)
				grounded = true;
			
			float dotWall = Vector3.Dot(c.normal, Vector3.right); 
			
			//if(Mathf.Abs(dotWall) > 0.5f)
			//	wallCollision = Mathf.Sign(dotWall);
		}
	}
	
	void OnCollisionExit2D(Collision2D col)
	{

		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			ContactPoint2D c = col.contacts[i];
			
			float dotWall = Vector3.Dot(c.normal, Vector3.right); 
			
			if(Mathf.Abs(dotWall) > 0.5f)
				wallCollision = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Trigger t = col.GetComponent<Trigger>();

		if(t && !triggers.Contains(t))
			triggers.Add(t);
	}

	void OnTriggerExit2D(Collider2D col)
	{
		Trigger t = col.GetComponent<Trigger>();
		
		if(t && triggers.Contains(t))
			triggers.Remove(t);
	}

}