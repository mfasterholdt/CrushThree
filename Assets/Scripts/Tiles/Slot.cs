using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slot : Tile 
{
	public CollisionEvents entryTrigger;

	public TileCandy currentTile;

	public Animation anim;
	public AnimationClip animClose;
	public AnimationClip animOpen;

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
			PlaceTile(tile);
	}

	public bool PlaceTile(TileCandy tile)
	{
		if(currentTile) 
			return false;

		PlayAnimation(animClose);
		currentTile = tile;
		tile.SetSlotState(pos);

		return true;
	}

	public void RemoveTile()
	{
		PlayAnimation(animOpen);
		currentTile = null;
	}

	public bool IsElectric()
	{
		if(currentTile && currentTile.type == TileCandy.CandyType.Electro)
			return true;
		else
			return false;
	}

	public override void FixedUpdate()
	{
		if(currentTile == null)
			return;

		if(!currentTile.rigidbody2D.isKinematic)
			RemoveTile();
	}

	private void PlayAnimation(AnimationClip clip, float blendSpeed = 0.2f)
	{
		if(anim && clip)
			anim.CrossFade(clip.name, blendSpeed);
	}
}
