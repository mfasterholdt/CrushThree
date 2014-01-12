using UnityEngine;
using System.Collections;

public class TileCandySpawner : Tile
{
	public Tile[] tilePrefabs;
	public float force = 7;

	private Tile target;
	private Vector2int spawnTarget;
	
	void Start () 
	{
		float rot = Mathf.RoundToInt(visuals.transform.rotation.eulerAngles.z / 90f); 

		if(rot == 0)
			dir = Vector2int.down;
		else if(rot == 1)
			dir = Vector2int.right;
		else if(rot == 2)
			dir = Vector2int.up;
		else if(rot == 3)
			dir = Vector2int.left;

		spawnTarget = pos + dir;
	}

	int type;

	void Spawn()
	{
		if(target == null || target.pos != spawnTarget)
		{
			Tile tile = Level.Instance.GetTile(spawnTarget);

			if(tile)
			{
				target = tile;
			}
			else
			{
				type = Random.Range(0, tilePrefabs.Length);

				target = Level.Instance.CreateTile(spawnTarget, tilePrefabs[type].gameObject);
				target.AddVelocity(dir.ToVector3() * force);
			}
		}
	}

	void FixedUpdate () 
	{
		Spawn ();
	}
}
