using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerList : MonoBehaviour 
{
	public List<Slot> slots;  
	public List<TileCandy> staticTiles;

	private List<TileCandy> tiles = new List<TileCandy>();

	public TileCandy GetSpawnTile()
	{
		tiles.Clear();

		tiles.AddRange(staticTiles);

		for(int i = 0, count = slots.Count; i < count; i++)
		{
			Slot slot = slots[i];

			if(slot.currentTile)
				tiles.Add(slot.currentTile);
		}

		if(tiles.Count == 0) 
			return null;


		int index = Random.Range(0, tiles.Count);

		return tiles[index];
	}
}
