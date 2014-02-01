using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slot : Tile 
{
	public CollisionEvents entryTrigger;

	public TileCandy currentTile;
	
	public override void Start () 
	{
		base.Start();

		entryTrigger.OnTriggerEnter += OnEntryEnter;
	}
	
	void OnEntryEnter(Collider2D col)
	{
		if(currentTile) 
			return;

		TileCandy tile = col.GetComponent<TileCandy>();

		if(tile) 
		{
			currentTile = tile;
			tile.SetSlotState(pos);
		}
	}

	void FixedUpdate()
	{
		if(currentTile == null)
			return;

		if(!currentTile.rigidbody2D.isKinematic)
			currentTile = null;
	}
}
