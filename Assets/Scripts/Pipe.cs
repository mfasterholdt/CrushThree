using UnityEngine;
using System.Collections;

public class Pipe : WorldObject
{
	public Transform entryPos;
	public Transform exitPos;

	private TileCandy[] content;

	void Start () 
	{
		int length = (int)Mathf.Abs(exitPos.position.x - entryPos.position.x) + (int)Mathf.Abs(exitPos.position.y - entryPos.position.y);
		content = new TileCandy[length];
	}

	void InjectTile(TileCandy tile)
	{
		//***Content should be a collision list

		//***Possible call tiles to secure execution order

		//***Possible have a helper function for moving tiles along

		if(content[0] == null)
		{
			content[0] = tile;
		}
	}

	void Update () 
	{
		for(int i = 0, count = content.Length; i < count; i++)
		{

		}
	}
}
