using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotMatch : Tile 
{
	public CollisionEvents entryTrigger;
	public TileCandy.CandyType type;

	public List<TileCandy> tiles;
	public GameObject[] objectsToClear;

	private bool complete;

	public override void Start () 
	{
		base.Start();

		entryTrigger.OnTriggerEnter += OnEntryEnter;
		entryTrigger.OnTriggerExit += OnEntryExit;
	}

	void FixedUpdate()
	{
		if(complete)
			return;

		for(int i = 0, count = tiles.Count; i<count; i++)
		{
			TileCandy tile = tiles[i];

			if(Mathf.Abs(tile.transform.position.x - transform.position.x) < 0.25f && Mathf.Abs(tile.transform.position.y - transform.position.y) < 0.25f)
				MatchComplete(tile);
		}
	}

	void MatchComplete(TileCandy tile)
	{
		complete = true;

		tiles.Clear();

		for(int i = 0, count = objectsToClear.Length; i < count; i++)
		{
			GameObject obj = objectsToClear[i];
			Destroy(tile.gameObject);
			Destroy(obj);
		}

		entryTrigger.OnTriggerEnter -= OnEntryEnter;
		entryTrigger.OnTriggerExit -= OnEntryExit;
	}

	void OnEntryEnter(Collider2D col)
	{
		TileCandy tile = col.GetComponent<TileCandy>();

		if(tile && tile.type == type && !tiles.Contains(tile)) 
			tiles.Add(tile);
	}

	void OnEntryExit(Collider2D col)
	{
		TileCandy tile = col.GetComponent<TileCandy>();

		if(tile && tiles.Contains(tile))
			tiles.Remove(tile);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, Vector3.one);
	}
}
