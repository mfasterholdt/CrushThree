using UnityEngine;


public class Tile : WorldObject 
{
	[HideInInspector]
	public Vector2int pos;
	
	public GameObject visuals;
	public bool debug;

	public Vector3 targetPos;
	protected Vector2int dir;
	
	protected Vector2int spawnForce;
	public Vector3 velocity; 

	protected float moveSpeed = 13f;
	protected float acceleration = 50f;

	public bool ground;

	public virtual void Initialize () 
	{
		pos = new Vector2int(transform.position.x, transform.position.y);
		
		spawnForce = Vector2int.zero;
		
		targetPos = transform.position;
	}

	public virtual void MoveTile(Vector2int p)
	{
		//Visual move target
		Vector3 newPos = p.ToVector3();
		targetPos = newPos;

		//Logic move
		pos = p;
	}

	public virtual void FixedUpdate()
	{
		if(spawnForce != Vector2int.zero && transform.position == targetPos)
		{
			spawnForce = new Vector2int(0, -1);
			Vector2int moveTarget = pos + spawnForce;

			Tile tile = Level.Instance.GetTile(moveTarget);

			if(tile == null)
			{
				Level.Instance.MoveTile(this, moveTarget);
			}
		}
	}

	public void AddVelocity(Vector3 add)
	{
		velocity += add;
	}

	public virtual void Update()
	{
		if(targetPos != transform.position)
		{
			Vector3 diff = (targetPos - transform.position);

			//Accelerate
			velocity += diff.normalized * Time.deltaTime * acceleration;

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
				else
				{
					velocity.x = 0;
				}
			}

			transform.position = nextPos;
		}
	}

	public bool CheckGround(int x, int y)
	{
		Tile tile = Level.Instance.GetTile(x, y - 1);

		if(tile == null) 
		{
			return false;
		}
		else if(tile.ground) 
		{
			return true;
		}
		else
		{
			return CheckGround(x, y -1);
		}
	}

	public virtual void Landing()
	{
	}

	public virtual void Remove()
	{
		Destroy(gameObject);
	}
}
