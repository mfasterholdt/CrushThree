using UnityEngine;
using System.Collections;
using System.Linq;

public class Spawner : WorldObject
{
	public Tile[] tilePrefabs;

	private TileCandy target;

	private Vector3 spawnTarget;
	private Vector2int dir;

	private int type;

	bool init;

	void Start()
	{
		SetSpawnTarget();
	}

	void SetSpawnTarget()
	{
		float rot = Mathf.RoundToInt(transform.rotation.eulerAngles.z / 90f); 

		if(rot == 0)
			dir = Vector2int.down;
		else if(rot == 1)
			dir = Vector2int.right;
		else if(rot == 2)
			dir = Vector2int.up;
		else if(rot == 3)
			dir = Vector2int.left;
		
		spawnTarget = transform.position + dir.ToVector3();
	}

	float TileDist()
	{
		float dist = Mathf.Abs(target.transform.position.x - spawnTarget.x) + Mathf.Abs(target.transform.position.y - spawnTarget.y);

		return dist;
	}

	void Spawn()
	{
		if(target == null || TileDist() >= 1)
		{
			int type = Random.Range(0, tilePrefabs.Length);
						
			GameObject newTile = Instantiate(tilePrefabs[type].gameObject, spawnTarget, Quaternion.identity) as GameObject;

			target = newTile.GetComponent<TileCandy>();
		}

		/*if(target == null || target.pos != spawnTarget)
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
		}*/
	}

	void FixedUpdate ()
	{
		Spawn ();
	}
}
