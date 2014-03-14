using UnityEngine;
using System.Collections;

public class Splitter : WorldObject, IConnectable
{
	public Board board;
	public Slot powerSource;

	int rightIndex;
	int leftIndex;
	int heightIndex;

	void Start () 
	{
		rightIndex = (int)(board.transform.position.x + board.width - 1);
		leftIndex = (int)(board.transform.position.x);
		heightIndex = (int)board.transform.position.y + board.height;
	}

	public bool RecieveCheck(TileCandy tile = null)
	{
		return true;
	}

	public bool ParseTile(TileCandy tile)
	{
		if(powerSource && !powerSource.IsElectric())
			return false;

		for(int i = leftIndex; i <= rightIndex; i++)
		{
			bool success = Level.Instance.PlaceTile(i, heightIndex, tile);
			
			if(success)
				return true;
		}
		
		return false;
	}

	public Vector3 GetConnectionPos()
	{
		return transform.position;
	}
}
