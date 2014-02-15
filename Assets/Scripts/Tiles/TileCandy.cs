using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCandy : Tile 
{
	public enum CandyType{ Apple, Banana, Grape, Ruby, Emerald, Diamond, BlueBerry, Electro, Black};

	//Properti Settings
	public CandyType type;
	
	private Pipe currentPipe;

	public bool glitch;
	private Material initialMaterial;
	private Material glitchMaterial;
	private float glitchTimer;

	//Physics Settings
	private float gravity = -15f; //Rigidbody gravity
	
	//Grid Settings
	protected Vector3 velocity; 
	protected float moveSpeedMax = 22f;
	protected float acceleration = 50f;


	public override void Start()
	{
		base.Start();

		if(rigidbody2D && !rigidbody2D.isKinematic)
			SetIdleState();
	}

	//--//States//--//

	//--//Board
	public void SetBoardState()
	{
		if(rigidbody2D)
			rigidbody2D.isKinematic = true;

		/*if(collider2D)
			collider2D.enabled = false;*/

		state.SetState(BoardState, BoardStateVisual);
	}

	private void BoardState()
	{
		//Falling
		if(transform.position == targetPos)
		{
			Vector2int moveTarget = pos + Vector2int.down;
			
			Tile tile = Level.Instance.GetTile(moveTarget);
			
			if(tile == null)
				Level.Instance.MoveTile(this, moveTarget);
		}
	}

	private void BoardStateVisual()
	{
		MoveTowardsTarget(true, false);

		//Glitch blinking
		if(glitchTimer > 0)
		{
			glitchTimer -= Time.deltaTime;
			
			if(glitchTimer <= 0)
			{
				if(visuals.renderer.material == initialMaterial)
				{
					visuals.renderer.material = glitchMaterial;
					glitchTimer = Random.Range(0.1f, 0.3f); 
					//glitchTimer = 1f; 
				}
				else
				{
					visuals.renderer.material = initialMaterial;
					glitchTimer = Random.Range(1f, 3f);
					//glitchTimer = 1f; 
				}
			}
		}
	}

	//--//Idle
	public void SetIdleState()
	{
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = false;
		rigidbody2D.collider2D.enabled = true;
		rigidbody2D.collider2D.isTrigger = false;

		state.SetState(IdleState, null);
	}

	private void IdleState()
	{
		//Gravity
		if(rigidbody2D)
			rigidbody2D.AddForce(Vector2.up * gravity);
	}

	//--//Carry
	public void SetCarryState()
	{
		if(state.FixedUpdate == BoardState)
			Level.Instance.RemoveTile(this, false);

		rigidbody2D.isKinematic = false;

		state.SetState(CarryState, null);
	}

	private void CarryState()
	{

	}

	//--//Pipe
	public void SetPipeState(Pipe pipe, Vector2int pipeStart)
	{
		currentPipe = pipe;

		rigidbody2D.isKinematic = true;
		velocity = Vector3.zero;
		RoundPosition();

		MoveTile(pipeStart);

		state.SetState(PipeState, PipeStateVisual);
	}

	private void PipeState()
	{
		//When target position is reached, attempt to move to next tile
		if(transform.position == targetPos)
		{
			bool tileMoved = currentPipe.MoveTile(this, pos + currentPipe.dir);

			if(!tileMoved)
			{
				velocity = Vector3.zero;

				if(currentPipe.connectedTo)
				{
					currentPipe.CheckConnector(this);
				}
				else
				{
					if(currentPipe.CheckEnd(this))
						SetIdleState();
				}
			}
		}
	}

	private void PipeStateVisual()
	{
		MoveTowardsTarget();
	}

	//--//Slot
	public void SetSlotState(Vector2int p)
	{
		MoveTile(p);

		rigidbody2D.isKinematic = true;

		velocity = Vector3.zero;

		state.SetState(SlotState, SlotStateVisual);
	}

	public void SlotState()
	{
	
	}

	public void SlotStateVisual()
	{
		MoveTowardsTarget();
	}

	//--//Helper Functions//--//
	
	public virtual void MoveTowardsTarget(bool breakX = false, bool breakY = false)
	{
		if(targetPos != transform.position)
		{
			Vector3 diff = (targetPos - transform.position);
			
			//Accelerate
			velocity += diff.normalized * Time.deltaTime * acceleration;
			
			velocity.x = Mathf.Min(velocity.x, moveSpeedMax);
			velocity.y = Mathf.Min(velocity.y, moveSpeedMax);
			
			//Predict
			Vector3 nextPos = transform.position + velocity * Time.deltaTime;
			
			bool overshootX = Mathf.Sign(diff.x) != Mathf.Sign(targetPos.x - nextPos.x);
			bool overshootY = Mathf.Sign(diff.y) != Mathf.Sign(targetPos.y - nextPos.y);
			
			if(overshootX || overshootY)
			{
				nextPos = targetPos;
				
				//Check underneath
				bool isGrounded = CheckGround(pos.x, pos.y);
				
				if(isGrounded)
				{
					velocity = Vector3.zero;
					Landing();
				}
				else if(breakX)
					velocity.x = 0;
				else if(breakY)
					velocity.y = 0;
			}
			
			transform.position = nextPos;
		}
	}

	public void AddVelocity(Vector3 add)
	{
		velocity += add;
	}

	public void Landing ()
	{
		Level.Instance.Match(this);
	}

	public void MatchTransition(float per)
	{
		visuals.transform.localScale = Vector3.one * per;
	}

	public void BecomeGlitch()
	{
		glitch = true;

		initialMaterial = visuals.renderer.material;
		glitchMaterial = Level.Instance.glitchMaterial;

		glitchTimer = Random.Range(1f, 3f);	

		Level.Instance.glitches.Add(this);
	}

	public void BecomeNormal()
	{
		glitch = false;

		visuals.renderer.material = initialMaterial;
		glitchTimer = 0;

		Level.Instance.glitches.Remove(this);
	}
}
