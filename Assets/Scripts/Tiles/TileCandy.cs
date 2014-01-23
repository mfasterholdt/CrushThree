using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCandy : Tile 
{
	public enum CandyType{ Apple, Banana, Grape, Ruby, Emerald, Diamond, BlueBerry};

	public CandyType type;

	public bool glitch;

	private Material initialMaterial;
	private Material glitchMaterial;
	private float glitchTimer;

	private float gravity = -15f;

	private delegate void State();
	State state;

	public void Start()
	{
		if(state == null)
			SetIdleState();
	}

	public override void Initialize()
	{
		base.Initialize();

		SetBoardState();
	}

	public void SetBoardState()
	{
		if(rigidbody2D)
			rigidbody2D.isKinematic = true;

		if(collider2D)
			collider2D.enabled = false;

		//Gravity
		spawnForce = new Vector2int(0, -1);

		state = BoardState;
	}

	private void BoardState()
	{
		base.FixedUpdate ();
		
		if(glitchTimer > 0)
		{
			glitchTimer -= Time.deltaTime;
			
			if(glitchTimer <= 0)
			{
				if(visuals.renderer.material == initialMaterial)
				{
					visuals.renderer.material = glitchMaterial;
					glitchTimer = 1f; //Random.Range(0.1f, 0.3f); 
				}
				else
				{
					visuals.renderer.material = initialMaterial;
					glitchTimer = 1f; //Random.Range(1f, 3f);
				}
			}
		}
	}
	
	public void SetIdleState()
	{
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = false;

		state = IdleState;
	}

	private void IdleState()
	{
		//Gravity
		if(rigidbody2D)
			rigidbody2D.AddForce(Vector2.up * gravity);
	}

	public void SetCarryState()
	{
		state = CarryState;
	}

	private void CarryState()
	{

	}

	private Pipe currentPipe;

	public void SetPipeState(Pipe pipe, Vector2int pipeStart)
	{
		currentPipe = pipe;

		rigidbody2D.isKinematic = true;

		velocity = Vector3.zero;

		MoveTile(pipeStart);

		state = PipeState;
	}

	private void PipeState()
	{
		if(transform.position == targetPos)
		{
			Vector2int moveTarget = pos + currentPipe.dir;

			Tile tile = currentPipe.GetTile(this, moveTarget);
			
			if(!tile)
				currentPipe.MoveTile(this, moveTarget);
			else
				velocity = Vector3.zero;
		}
	}

	public override void Update ()
	{
		if(state == BoardState || state == PipeState)
			base.Update ();
	}

	public override void FixedUpdate()
	{
		if(state != null)
			state();
	}

	public override void Landing ()
	{
		base.Landing ();

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
}
