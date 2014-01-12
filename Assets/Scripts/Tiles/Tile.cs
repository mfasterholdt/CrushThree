using UnityEngine;


public class Tile : WorldObject 
{
	[HideInInspector]
	public Vector2int pos;
	
	public GameObject visuals;
	public bool debug;

	protected Vector3 targetPos;
	protected Vector2int dir;
	
	protected Vector2int force;
	protected Vector3 velocity; 

	protected float moveSpeed = 13f;
	protected float acceleration = 40f;

	public virtual void Initialize () 
	{
		pos = new Vector2int(transform.position.x, transform.position.y);
		
		force = Vector2int.zero;
		
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
		if(force != Vector2int.zero && transform.position == targetPos)
		{
			force = new Vector2int(0, -1);
			Vector2int moveTarget = pos + force;

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
				Vector2int checkTarget = pos;
				checkTarget.y -= 1;
				Tile tile = Level.Instance.GetTile(checkTarget);

				if(Mathf.Sign(velocity.y) == 1 || (tile != null && tile.velocity.y == 0))
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

	public virtual void Landing()
	{
	}

	public virtual void Remove()
	{
		Destroy(gameObject);
	}
}
