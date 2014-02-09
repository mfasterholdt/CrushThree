using UnityEngine;
using System.Collections;
using System.Linq;

public class Spawner : WorldObject
{
	public SpawnItem[] items;
	public SpawnerList spawnerList;
	public WorldObject connectedTo;
	public Slot powerSource;

	private TileCandy target;

	private Vector3 spawnTarget;
	private Vector2int dir;

	private int type;

	bool init;

	int total;

	void Start()
	{
		if(connectedTo != null && !(connectedTo is IConnectable))
			Debug.LogError("illegal connection", gameObject);

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

	void Spawn()
	{
		if(powerSource && !powerSource.IsElectric())
			return;

		if(connectedTo)
		{
			//Connected
			IConnectable c = connectedTo as IConnectable;

			if(c == null || !c.RecieveCheck())
				return;

			Vector3 pos = c.GetConnectionPos();
			TileCandy newTile = SpawnNextItem(pos);

			c.ParseTile(newTile);

		}
		else if(target == null || TileDist() >= 0.75f)
		{
			//Spawn Frees
			SetSpawnTarget();

			target = SpawnNextItem(spawnTarget);

			target.SetIdleState();
		}
	}

	TileCandy SpawnNextItem(Vector3 pos)
	{
		Tile nextItem = null;

		if(spawnerList)
		{
			nextItem = spawnerList.GetSpawnTile() as Tile;
		}
		else
		{
			//Choose Item
			int type = Random.Range(0, total);
			
			for(int i = 0, count = items.Length; i < count; i++)
			{
				SpawnItem item = items[i];
				
				type -= item.ratio;
				
				if(type < 0)
				{
					nextItem = item.tilePrefab;
					break;
				}
			}
		}

		//Item found?
		if(nextItem == null)
			return null;


		//Spawn Item
		GameObject newObj = Instantiate(nextItem.gameObject, pos, Quaternion.identity) as GameObject;
		newObj.transform.parent = transform.parent;
		return newObj.GetComponent<TileCandy>();
	}

	void FixedUpdate ()
	{
		Spawn ();
	}
	
	float TileDist()
	{
		float dist = Mathf.Abs(target.transform.position.x - spawnTarget.x) + Mathf.Abs(target.transform.position.y - spawnTarget.y);

		return dist;
	}

	[System.Serializable]
	public class SpawnItem
	{
		public Tile tilePrefab;
		public int ratio;
	}
}
