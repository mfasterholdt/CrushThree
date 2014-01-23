using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pipe : WorldObject
{
	public CollisionEvents entryTrigger;
	public CollisionEvents endTrigger;

	public WorldObject connector;

	[HideInInspector]
	public Vector2int dir;
	[HideInInspector]
	public Vector2int pos;

	private Vector3 entryPos;
	private Vector3 endPos;
	
	private TileCandy[] content;

	private TileCandy exitTile;

	private List<Collider2D> tilesAtEntry = new List<Collider2D>();
	private List<Collider2D> tileAtEnd = new List<Collider2D>();

	void Start () 
	{
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
		//Inject tiles at entry?
		if(tilesAtEntry.Count > 0)
		{
			for(int i = 0, count = tilesAtEntry.Count; i < count; i++)
			{
				Collider2D col = tilesAtEntry[i];

				float dist = Mathf.Abs(entryPos.x - col.transform.position.x) + Mathf.Abs(entryPos.y - col.transform.position.y);

				if(dist < 0.2f)
				{
					TileCandy tile = col.GetComponent<TileCandy>(); 

					if(tile)
						AttemptInject(tile);
				}
			}
		}
	}

	public void MoveTile(Tile tile, Vector2int target)
	{
		Vector2int previousPos = pos - tile.pos;
		int previousIndex = dir.y != 0 ? previousPos.y : previousPos.x;
		content[previousIndex] = null;

		Vector2int targetPos = pos - target;
		int targetIndex = dir.y != 0 ? targetPos.y : targetPos.x;
		content[targetIndex] = tile as TileCandy;

		tile.MoveTile(target);
	}

	public Tile GetTile(TileCandy tile, Vector2int target)
	{
		Vector2int pipePos = pos - target;

		int index = dir.y != 0 ? pipePos.y : pipePos.x;

		if(connector)
		{
			if(index >= content.Length)
			{
				Splitter splitter = connector.GetComponent<Splitter>();

				if(splitter)
				{
					if(splitter.NeedTileCheck(tile))
						content[content.Length-1] = null;
				}
				else
				{

					Pipe pipe = connector.GetComponent<Pipe>();

					if(pipe)
					{
						if(pipe.AttemptInject(tile))
							content[content.Length-1] = null;
					}
				}

				return Level.BorderTile;
			}
		}
		else
		{
			if(index >= content.Length - 1)
			{
				if(tileAtEnd.Count == 0)
				{
					tile.SetIdleState();
					return null;
				}
				else
				{
					return Level.BorderTile;
				}
			}
		}

		return content[index];
	}

	void OnEndExit(Collider2D col)
	{
		tileAtEnd.Remove(col);
	}
	
	void OnEndEnter(Collider2D col)
	{
		tileAtEnd.Add(col);
	}

	void OnEntryExit(Collider2D col)
	{
		tilesAtEntry.Remove(col);
	}

	void OnEntryEnter(Collider2D col)
	{
		tilesAtEntry.Add(col);
	}
}
