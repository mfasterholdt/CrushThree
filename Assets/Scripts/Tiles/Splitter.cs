using UnityEngine;
using System.Collections;

public class Splitter : WorldObject
{
	public Board board;

	Vector3 testPos;

	int rightIndex;
	int leftIndex;
	int heightIndex;

	void Start () 
	{
		rightIndex = (int)(board.transform.position.x + board.width - 1);
		leftIndex = (int)(board.transform.position.x);
		heightIndex = (int)board.transform.position.y + board.height + 1;
	}

	public bool NeedTileCheck(TileCandy tile)
	{

		for(int i = leftIndex; i <= rightIndex; i++)
		{
			bool success = Level.Instance.PlaceTile(i, heightIndex, tile);

			if(success)
				return true;
		}

		return false;
	}
}
