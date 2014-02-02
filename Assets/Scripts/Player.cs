using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{	
	public Transform visuals;

	public float moveSpeed = 275f;
	public float airControl = 0.7f;
	public float climbSpeed = 5f;
	public float jumpForce = 50f;
	public float gravity = -15f;

	public Vector3 cameraOffset;
	private Camera cam;

	private bool grounded = true;
	private float vel;

	private bool camFollow;
	private float cameraEaseIn;
	private float wallCollision;
	
	List<Trigger> triggers = new List<Trigger>();

	public LayerMask environmentMask;
	//****Create STATES to handle all the stuffs!!!

	public Animation anim;
	public AnimationClip animIdle;
	public AnimationClip animJump;

	public State state = new State();

	[HideInInspector]
	public TileCandy carrying;

	[HideInInspector]
	public float facing = 1;

	void Start()
	{
		cam = Camera.main;

		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;

		SetMoveState();
	}

	//--//State

	//Move
	void SetMoveState()
	{
		anim.CrossFade(animIdle.name, 0.2f);
		state.SetState(MoveState, MoveStateVisual);
	}

	void MoveState()
	{
		//Movement
		BasicMovement(moveSpeed);

		//Climbing
		float inputY = Input.GetAxis("Vertical");
		
		if(triggers.Count > 0 && Mathf.Abs(inputY) > 0.2f)
			SetLadderState();

		//Gravity
		rigidbody2D.AddForce(Vector2.up * gravity);
	}

	void MoveStateVisual()
	{
		//Jump
		if(grounded && Input.GetKey(KeyCode.Space))
			SetJumpState();
		
		//Pickup
		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			AttemptPickup();
		
		//Drop
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			AttemptDrop();
	}

	//Jump
	void SetJumpState()
	{
		rigidbody2D.AddForce(Vector2.up * jumpForce);
		grounded = false;

		anim.CrossFadeQueued(animJump.name, 0.2f, QueueMode.PlayNow);

		state.SetState(JumpState, JumpStateVisual);
	}

	void JumpState()
	{
		BasicMovement(moveSpeed * airControl);

		//Climbing
		float inputY = Input.GetAxis("Vertical");
		
		if(triggers.Count > 0 && Mathf.Abs(inputY) > 0.2f)
			SetLadderState();
	
		//Land
		if(grounded)
			SetMoveState();

		//Gravity
		rigidbody2D.AddForce(Vector2.up * gravity);
	}

	void JumpStateVisual()
	{
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
	}

	//Ladder State
	void SetLadderState()
	{
		anim.CrossFade(animIdle.name, 0.2f);
		state.SetState(LadderState, null);
	}

	void LadderState()
	{
		BasicMovement(moveSpeed);	

		float inputY = Input.GetAxis("Vertical");
		Vector3 vel = rigidbody2D.velocity;

		vel.y = Time.deltaTime * inputY * climbSpeed;				
		rigidbody2D.velocity = vel;

		//Leave ladder
		if(triggers.Count == 0)
			SetMoveState();
	}

	//General Updates
	void FixedUpdate()
	{		
		if(state.FixedUpdate != null)
			state.FixedUpdate();
	}

	void Update()
	{
		if(state.Update != null)
			state.Update();

		//Rotating
		Vector3 scale = visuals.localScale;
		scale.x = facing;
		visuals.localScale = scale;

		//Carry
		if(carrying)
			carrying.transform.position = transform.position + transform.up * 1.4f;

		//Camera Follow
		Vector3 camTarget = transform.position - transform.forward * cameraOffset.z + transform.up * cameraOffset.y;
		cam.transform.position = camTarget;
	}

	//--//Helper functions

	void BasicMovement(float speed)
	{
		float inputX = Input.GetAxis("Horizontal");

		//No input
		if(inputX == 0)
			return;

		//Wall Collision
		Ray2D wallRay = new Ray2D(transform.position + Vector3.up * -0.8f, Vector3.right * Mathf.Sign(inputX));
		RaycastHit2D wallHit = Physics2D.Raycast(wallRay.origin, wallRay.direction, 0.3f, environmentMask);
		//Debug.DrawLine(wallRay.origin, wallRay.origin + wallRay.direction * 0.45f);
	
		Vector3 vel = rigidbody2D.velocity;			

		if(wallHit.collider) 		
			vel.x = 0;
		else
			vel.x = Time.deltaTime * inputX * speed;		

		rigidbody2D.velocity = vel;

		//Facing
		if(vel.x != 0)
			facing = Mathf.Sign(inputX);
	}

	void AttemptPickup()
	{
		//Already carrying an object
		if(carrying) return;

		RaycastHit2D hit = Physics2D.Raycast(transform.position, visuals.right * facing, 1f, environmentMask);

		//No collision found
		if(!hit) return;

		TileCandy tile = hit.collider.gameObject.GetComponent<TileCandy>();

		//Object cannot be carried
		if(!tile) return;

		//Pickup Object
		carrying = tile;

		tile.SetCarryState();
	}

	void AttemptDrop()
	{
		if(!carrying) return;

		RaycastHit2D hit = Physics2D.Raycast(transform.position, visuals.right * facing, 1f, environmentMask);
		
		if(hit) return;

		Vector3 dropPos = transform.position + visuals.right * facing;

		carrying.transform.position = dropPos;

		carrying.SetIdleState();

		carrying = null;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			ContactPoint2D c = col.contacts[i];

			//Ground
			float dot = Vector3.Dot(c.normal, Vector3.up); 
			
			if(dot > 0.6f)
				grounded = true;

			//Walls
			/*float dotWall = Vector3.Dot(c.normal, Vector3.right); 

			if(Mathf.Abs(dotWall) > 0.5f)
				wallCollision = Mathf.Sign(dotWall);*/
		}
	}
	
	void OnCollisionExit2D(Collision2D col)
	{

		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			/*ContactPoint2D c = col.contacts[i];
			
			float dotWall = Vector3.Dot(c.normal, Vector3.right); 
			
			if(Mathf.Abs(dotWall) > 0.5f)
				wallCollision = 0;*/
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