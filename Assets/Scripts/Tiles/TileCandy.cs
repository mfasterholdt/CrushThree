using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCandy : Tile 
{
	public enum CandyType{ Apple, Banana, Grape, Ruby, Emerald, Diamond};

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

	public void SetPipeState(Pipe p, Vector2int pos)
	{
		currentPipe = p;

		//***Set pipe pose as pos

		state = PipeState;
	}

	private void PipeState()
	{
		//***look up in pipe to check for collision

		//***possibly use old move of visuals again
	}

	public override void Update ()
	{
		if(state == BoardState)
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
