using UnityEngine;
using System.Collections;
using System.Linq;

public class Spawner : WorldObject
{
	public SpawnItem[] items;

	private TileCandy target;

	private Vector3 spawnTarget;
	private Vector2int dir;

	private int type;

	bool init;

	int total;

	void Start()
	{
		SetSpawnTarget();

		for(int i = 0, count = items.Length; i < count; i++)
		{
			SpawnItem item = items[i];

			total += item.ratio;
		}
	}

	void SetSpawnTarget()
	{
		dir = new Vector2int(-transform.up.x, -transform.up.y);

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
			int type = Random.Range(0, total);

			SpawnItem nextItem = null;

			for(int i = 0, count = items.Length; i < count; i++)
			{
				SpawnItem item = items[i];
				
				type -= item.ratio;

				if(type < 0)
				{
					nextItem = item;
					break;
				}
			}

			if(nextItem != null)
			{
				GameObject newTile = Instantiate(nextItem.tilePrefab.gameObject, spawnTarget, Quaternion.identity) as GameObject;

				target = newTile.GetComponent<TileCandy>();
			}
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

	[System.Serializable]
	public class SpawnItem
	{
		public Tile tilePrefab;
		public int ratio;
	}
}
