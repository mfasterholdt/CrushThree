using UnityEngine;


public class Tile : WorldObject 
{
	public GameObject visuals;
	public bool debug;
	public bool ground;

	[HideInInspector]
	public Vector2int pos;

	[HideInInspector]
	public Vector2int dir;	

	[HideInInspector]
	public Vector3 targetPos;	
	
	[HideInInspector]
	public State state = new State();

	public virtual void Start()
	{
		pos = new Vector2int(transform.position.x, transform.position.y);
		targetPos = transform.position;
	}
	
	public virtual void Update ()
	{
		if(state.Update != null)
			state.Update();
	}

	public virtual void FixedUpdate()
	{
		if(state.FixedUpdate != null)
			state.FixedUpdate();
	}

	//--//Helper Functions//--//

	public virtual void MoveTile(Vector2int p)
	{
		//Visual move target
		Vector3 newPos = p.ToVector3();
		targetPos = newPos;
		
		//Logic move
		pos = p;
	}

	protected virtual bool CheckGround(int x, int y)
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

	protected void RoundPosition()
	{
		Vector3 roundPos = transform.position;
		roundPos.x = Mathf.Round(roundPos.x);
		roundPos.y = Mathf.Round(roundPos.y);
		transform.position = roundPos;
	}

	public virtual void Remove()
	{
		Destroy(gameObject);
	}
}
