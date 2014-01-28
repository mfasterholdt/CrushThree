using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pipe : WorldObject, IConnectable
{
	public CollisionEvents entryTrigger;
	public CollisionEvents endTrigger;

	public WorldObject connectedTo;

	[HideInInspector]
	public Vector2int dir;

	[HideInInspector]
	public Vector2int pos;

	private Vector3 entryPos;
	private Vector3 endPos;
	
	private TileCandy[] content;

	private List<Collider2D> tilesAtEntry = new List<Collider2D>();
	private List<Collider2D> tileAtEnd = new List<Collider2D>();

	void Start () 
	{
		if(connectedTo != null && !(connectedTo is IConnectable))
			Debug.LogError("illegal connection", gameObject);

		pos = new Vector2int(transform.position.x, transform.position.y);

		entryPos = entryTrigger.transform.position;
		endPos = endTrigger.transform.position;

		int length = (int)Mathf.Abs(endPos.x - entryPos.x) + (int)Mathf.Abs(endPos.y - entryPos.y);
		content = new TileCandy[length];

		dir = new Vector2int(-transform.up.x, -transform.up.y);

		entryTrigger.OnTriggerEnter += OnEntryEnter;
		entryTrigger.OnTriggerExit += OnEntryExit;

		endTrigger.OnTriggerEnter += OnEndEnter;
		endTrigger.OnTriggerExit += OnEndExit;
	}

	bool AttemptInject(TileCandy tile)
	{
		if(content[0] == null)
		{
			content[0] = tile;
			tile.SetPipeState(this, pos); 

			return true;
		}
		else
		{
			return false;
		}
	}

	void FixedUpdate()
	{
		//Inject tiles at entry
		for(int i = 0, count = tilesAtEntry.Count; i < count; i++)
		{
			Collider2D col = tilesAtEntry[i];

			float dist = Mathf.Abs(entryPos.x - col.transform.position.x) + Mathf.Abs(entryPos.y - col.transform.position.y);

			if(dist < 0.1f)
			{
				TileCandy tile = col.GetComponent<TileCandy>(); 

				if(tile)
					AttemptInject(tile);
			}
		}
	}

	public bool MoveTile(TileCandy tile, Vector2int target)
	{
		//Is there a tile in the way?
		Tile targetTile = GetTile(tile, target);
		if(targetTile) return false;
	
		//Move tile
		content[PosToIndex(tile.pos)] = null;
		content[PosToIndex(target)] = tile as TileCandy;
		tile.MoveTile(target);

		return true;
	}
	
	public Tile GetTile(TileCandy tile, Vector2int target)
	{
		int index = PosToIndex(target);

		if(index == content.Length - 1)
		{
			if(tileAtEnd.Count != 0)
				return Level.BorderTile;
		}
		else if(index == content.Length)
		{
			return Level.BorderTile;
		}

		return content[index];
	}

	public bool CheckEnd(TileCandy tile)
	{
		int index = PosToIndex(tile.pos);

		//Not at the end
		if(index != content.Length -1)
			return false;

		content[index] = null;

		//Place in end list
		Collider2D col = tile.GetComponent<Collider2D>();
		if(col)
			OnEndEnter(col);

		return true;
	}

	public bool CheckConnector(TileCandy tile)
	{
		int index = PosToIndex(tile.pos);

		//Not at the end
		if(index != content.Length - 1)
			return false;

		IConnectable c = connectedTo as IConnectable;

		//No connectable
		if(c == null)
			return false; 

		//Ready to receive?
		if(!c.RecieveCheck()) 
			return false; 

		bool moved = c.ParseTile(tile);
		if(!moved)
			return false;

		//Move
		content[index] = null;

		return true;
	}

	public bool RecieveCheck(TileCandy tile = null)
	{
		return content[0] == null;
	}

	public bool ParseTile(TileCandy tile)
	{
		return AttemptInject(tile);
	}

	public Vector3 GetConnectionPos()
	{
		return transform.position;
	}

	//Converts world position to pipe index
	public int PosToIndex(Vector2int p)
	{
		Vector2int posDiff = pos - p;

		int index = (int)Mathf.Abs(posDiff.x) + Mathf.Abs(posDiff.y);

		return index;
	}
	
	void OnEndExit(Collider2D col)
	{
		tileAtEnd.Remove(col);
	}
	
	void OnEndEnter(Collider2D col)
	{
		if(!tileAtEnd.Contains(col))
			tileAtEnd.Add(col);
	}

	void OnEntryExit(Collider2D col)
	{
		tilesAtEntry.Remove(col);
	}

	void OnEntryEnter(Collider2D col)
	{
		if(!tilesAtEntry.Contains(col))
			tilesAtEntry.Add(col);
	}
}
