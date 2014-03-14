using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class Player : SingletonComponent<Player>
{	
	public Transform visuals;
	public Room currentRoom;

	public float moveSpeed = 100f;
	public float groundFriction = 0.1f;
	public float airMoveSpeed = 85f;
	public float airFriction = 0.1f;
	public float maxSpeed = 6f;

	public float climbSpeed = 350f;
	public float jumpForce = 725f;
	public float gravity = -42f;
	public float height = 1.2f;
	public Vector3 cameraOffset;
	private Camera cam;

	private bool grounded = true;
	private float vel;

	private bool camFollow;
	private float cameraEaseIn;
	private float wallCollision;
	private Entrance currentEntrance;

	List<Trigger> ladderTriggers = new List<Trigger>();

	public LayerMask environmentMask;

	public bool startInBoard = false;

	public Animation anim;
	public AnimationTheme themeDefault;
	public AnimationTheme themeCarry;
	private AnimationTheme currentTheme;

	public bool debug;

	public State state = new State();

	[HideInInspector]
	public TileCandy carrying;

	[HideInInspector]
	public float facing = 1;

	void Start()
	{
		currentTheme = themeDefault;

		currentRoom = FindObjectsOfType<Room>().ToList().Find(x => x.gameObject.activeSelf);

		if(!currentRoom) 
			Debug.LogWarning("No active room found");

		cam = Camera.main;

		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;

		if(startInBoard)
		{
			visuals.gameObject.SetActive(false);
			SetBoardState();
		}
		else 
			SetMoveState();

		//Set camerea on start
		cam.transform.position = transform.position - transform.forward * cameraOffset.z + transform.up * cameraOffset.y;
	}

	//--//State
	void SetBoardState()
	{
		//visuals.gameObject.SetActive(false);
		PlayAnimation(PlayerAnim.Idle);
		rigidbody2D.isKinematic = true;
		state.SetState(BoardState, null);
	}

	void BoardState()
	{
		//Debug temp
		if(debug && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
		{
			grounded = false;
			Level.Instance.glitchCount = 0;
			SetMoveState();
		}
	}

	//Move
	public void SetMoveState()
	{
		visuals.gameObject.SetActive(true);
		rigidbody2D.isKinematic = false;

		state.SetState(MoveState, MoveStateVisual);
	}

	void MoveState()
	{
		//Movement
		BasicMovement(moveSpeed, groundFriction);

		//Climbing
		float inputY = Input.GetAxis("Vertical");
		
		if(ladderTriggers.Count > 0 && Mathf.Abs(inputY) > 0.2f)
			SetLadderState();

		float inputX = GetHorizontalInput();

		if(inputX == 0)
			PlayAnimation(PlayerAnim.Idle);
		else
			PlayAnimation(PlayerAnim.Run);

		//Gravity
		rigidbody2D.AddForce(Vector2.up * gravity);
	}

	void MoveStateVisual()
	{
		//Jump
		if(grounded && Input.GetKey(KeyCode.Space))
			SetJumpState();
		

		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			//Pickup
			AttemptPickup();

			//Enter Room
			if(currentEntrance)
				EnterRoom(currentEntrance);
		}
		
		//Drop
		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			AttemptDrop();
	}

	//Jump
	void SetJumpState()
	{
		rigidbody2D.AddForce(Vector2.up * jumpForce);

		grounded = false;

		PlayAnimationQueued(PlayerAnim.Jump);

		state.SetState(JumpState, JumpStateVisual);
	}

	void JumpState()
	{
		BasicMovement(airMoveSpeed, airFriction);

		//Climbing
		float inputY = Input.GetAxis("Vertical");
		
		if(ladderTriggers.Count > 0 && Mathf.Abs(inputY) > 0.2f)
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
		PlayAnimation(PlayerAnim.Idle);
		state.SetState(LadderState, null);
		rigidbody2D.velocity = Vector2.zero;
	}

	void LadderState()
	{
		float inputY = Input.GetAxis("Vertical");

		if(Mathf.Abs (inputY) < 0.1f)
			BasicMovement(moveSpeed / 2f, groundFriction);	

		Vector3 vel = rigidbody2D.velocity;

		vel.y = Time.deltaTime * inputY * climbSpeed;				
		rigidbody2D.velocity = vel;

		//Leave ladder
		if(ladderTriggers.Count == 0)
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
			carrying.transform.position = transform.position + transform.up * height * 0.75f;

		//Camera Follow
		Vector3 camTarget = Vector3.zero;
		
		if(state.FixedUpdate == BoardState && Level.Instance != null)
			camTarget =  Level.Instance.board.transform.position + Vector3.right * 4.5f + Vector3.up * 2.5f - transform.forward * cameraOffset.z;
		else
			camTarget = transform.position - transform.forward * cameraOffset.z + transform.up * cameraOffset.y;
		
		Vector3 camPos = cam.transform.position;
		camPos += (camTarget - camPos) * Time.deltaTime * 3f;
		cam.transform.position = camPos;
	}
	
	//--//Helper functions
	private void PlayAnimation(PlayerAnim playerAnim, float blendTime = 0.2f)
	{
		AnimationClip clip = currentTheme.getAnim(playerAnim);

		if(anim && clip)
			anim.CrossFade(clip.name, blendTime);
	}

	private void PlayAnimationQueued(PlayerAnim playerAnim, float blendTime = 0.2f)
	{
		AnimationClip clip = currentTheme.getAnim(playerAnim);

		if(anim && clip)
			anim.CrossFadeQueued(clip.name, blendTime, QueueMode.PlayNow);			
	}

	private float GetHorizontalInput()
	{
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		   return -1f;
		else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		   return 1f;

		return 0;
	}

	void BasicMovement(float speed, float friction)
	{
		float inputX = GetHorizontalInput();

		//Wall Collision
		Ray2D wallRay = new Ray2D(transform.position + Vector3.up * height * -0.4f, Vector3.right * Mathf.Sign(inputX));
		RaycastHit2D wallHit = Physics2D.Raycast(wallRay.origin, wallRay.direction, 0.5f, environmentMask);

		//***Push should happen as add force instead of collider push
		if(debug)
			Debug.DrawLine(wallRay.origin, wallRay.origin + wallRay.direction * 0.5f, Color.red);

		Vector3 vel = rigidbody2D.velocity;			

		//Friction
		if(inputX == 0)
		{
			vel.x -= vel.x * friction;
			rigidbody2D.velocity = vel;
		}

		bool changeDirection = Mathf.Sign(inputX) != Mathf.Sign(vel.x);
	
		if(Mathf.Abs(vel.x) > maxSpeed && (inputX == 0 || !changeDirection))
		{
			//Max speed
			vel.x = Mathf.Sign(vel.x) * maxSpeed;
			rigidbody2D.velocity = vel;
		}

		if(wallHit.collider)
		{
			//Wall
			vel.x = 0;
			rigidbody2D.velocity = vel;
		}
		else
		{
			//Move
			rigidbody2D.AddForce(Vector2.right * inputX * speed);
		}


		//Facing
		if(inputX != 0)
			facing = Mathf.Sign(inputX);
	}

	void GroundFriction()
	{
		float inputX = GetHorizontalInput();

		if(inputX == 0)
			rigidbody2D.velocity -= rigidbody2D.velocity * groundFriction; 
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
		tile.transform.parent = transform;
		currentTheme = themeCarry;

		tile.SetCarryState();
	}

	void AttemptDrop()
	{
		if(!carrying) return;

		//Wall check
		RaycastHit2D hit = Physics2D.Raycast(transform.position, visuals.right * facing, 1f, environmentMask);
		if(hit) return;

		Vector3 dropPos = transform.position + visuals.right * facing;

		carrying.transform.position = dropPos;

		if(currentRoom)
			carrying.transform.parent = currentRoom.transform;
		else 
			carrying.transform.parent = transform.parent;

		carrying.SetIdleState();

		currentTheme = themeDefault;
		carrying = null;
	}
	
	public void EnterRoom (Entrance entrance) 
	{
		currentRoom.gameObject.SetActive(false);
		
		entrance.room.gameObject.SetActive(true);
		
		currentRoom = entrance.room;
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
		}
	}
	
	void OnCollisionExit2D(Collision2D col)
	{
		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
		}
	}


	void OnTriggerEnter2D(Collider2D col)
	{
		Entrance entrance = col.GetComponent<Entrance>();
		
		if(entrance) 
		{
			currentEntrance = entrance;
			return;
		}

		Trigger t = col.GetComponent<Trigger>();

		if(!t)
			return;

		if(t.type == Trigger.Type.Ladder && !ladderTriggers.Contains(t))
		{
			ladderTriggers.Add(t);
		}
		else if(t.type == Trigger.Type.MindHat && state.FixedUpdate == JumpState)
		{
			SetBoardState();
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(currentEntrance && col.GetComponent<Entrance>())
		{
			currentEntrance = null;
			return;
		}

		Trigger t = col.GetComponent<Trigger>();

		if(!t)
			return;

		if(t.type == Trigger.Type.Ladder && ladderTriggers.Contains(t))
		{
			ladderTriggers.Remove(t);
		}
	}
}