using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : WorldObject 
{
	public int width = 1;
	public int height = 1;

	public Tile[] tilePrefabs;

	private List<TileCandy> startBoard;

	public List<TileCandy> Initialize () 
	{
		startBoard = new List<TileCandy>();

		if(tilePrefabs.Length == 0) return null;

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

				TileCandy newTile = Level.Instance.CreateTile(pos, tilePrefabs[type].gameObject) as TileCandy;		

				startBoard.Add(newTile);
			}
		}

		return startBoard;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0.2f, 0.5f);

		Vector3 pos = transform.position;
		pos.x += width / 2 - 0.5f;
		pos.y += height / 2 - 0.5f;

		Gizmos.DrawWireCube(pos, Vector3.up * height + Vector3.right * width);
	}
}
