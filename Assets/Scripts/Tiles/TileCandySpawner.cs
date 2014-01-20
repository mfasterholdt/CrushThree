using UnityEngine;
using System.Collections;
using System.Linq;

public class TileCandySpawner : Tile
{
	public Tile[] tilePrefabs;
	public float spawnForce = 0;

	private Tile target;
	private Vector2int spawnTarget;
	private int type;

	bool init;

	public override void Initialize ()
	{
		base.Initialize ();
	
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

				if(Level.Instance.glitches.Count < Level.Instance.glitchCount)
				{
					if(Level.Instance.glitches.Find(x => (int)x.type == type) == null)
					{
						(target as TileCandy).BecomeGlitch();
					}
				}

				target.AddVelocity(dir.ToVector3() * spawnForce);
			}
		}
	}

	public override void FixedUpdate ()
	{
		Spawn ();
	}
}
