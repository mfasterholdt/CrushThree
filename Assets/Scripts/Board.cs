using UnityEngine;
using System.Collections;

public class Board : WorldObject 
{
	public int width = 1;
	public int height = 1;

	public Tile[] tilePrefabs;
	
	void Start () 
	{
		if(tilePrefabs.Length == 0) return;

		Vector2int origin = new Vector2int(transform.position.x, transform.position.y);
		Vector2int pos;

		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				pos = origin;
				pos.x += x;
				pos.y += y;

				int type = Random.Range(0, tilePrefabs.Length);

				Level.Instance.CreateTile(pos, tilePrefabs[type].gameObject);		
			}
		}
	}

	void Update () {
	
	}
}
