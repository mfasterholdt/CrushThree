using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level : SingletonComponent<Level> 
{
	[HideInInspector]
	public Selection selection;
	public GameObject selectionPrefab;
	public GameObject selectionTarget;

	public Tile borderTile;
	
	[HideInInspector]	
	public Tile[,] world;
	
	[HideInInspector]
	public List<Tile> tiles;
	
	[HideInInspector]
	public static int WorldSize = 300;
		
	private Tile pickedTile;
	
	public delegate void StepEvent();
	public event StepEvent OnStep;
	
	void Start () 
	{
		if(selectionPrefab)
		{
			GameObject selectionObj = Instantiate(selectionPrefab) as GameObject;
			selection = selectionObj.GetComponent<Selection>();
			selection.OnMouseClick += OnSelectionClick;
		}

		RegisterWorld();
	}
	
	void RegisterWorld()
	{
		world = new Tile[WorldSize, WorldSize];
		
		Tile[] preplaced = FindObjectsOfType(typeof(Tile)) as Tile[];
		
		for(int i=0, count = preplaced.Length; i<count; i++)
		{
			Tile tile = preplaced[i];
			tile.Initialize();
			
			Tile current = world[tile.pos.x, tile.pos.y];
			
			if(current != null)
			{
				Debug.LogError("Overlapping tiles "+current, current.gameObject);	
			}
			else
			{
				world[tile.pos.x, tile.pos.y] = tile;
				
				tiles.Add(tile);
			}
		}
	}
	
	void OnSelectionClick(Selection sender, Vector2int pos)
	{
		Tile tile = GetTile(pos);
		
		if(tile)
		{
			if(pickedTile)
			{
				if(tile != pickedTile)
				{
					//Swap
					bool success = SwapTiles(tile, pickedTile);

					if(success)
					{
						selectionTarget.SetActive(false);
						pickedTile = null;
					}
					else
					{
						Select(tile);
					}
				}
			}
			else
			{
				Select(tile);
			}
		}
	}

	void Select(Tile tile)
	{
		pickedTile = tile;

		selectionTarget.SetActive(true);
		selectionTarget.transform.position = tile.transform.position;
	}

	void Push(Tile tile, Vector2int force)
	{
		Vector2int p = tile.pos + force;
		
		//***Add collision check
		
		MoveTile(tile, p);
	}

	private List<TileCandy> horizontalMatches = new List<TileCandy>();
	private List<TileCandy> verticalMatches = new List<TileCandy>();

	public void Match(TileCandy tile)
	{
		horizontalMatches.Clear();
		verticalMatches.Clear();

		Connect(horizontalMatches, tile.type, tile.pos, Vector2int.left);
		Connect(horizontalMatches, tile.type, tile.pos, Vector2int.right);

		Connect(verticalMatches, tile.type, tile.pos, Vector2int.up);
		Connect(verticalMatches, tile.type, tile.pos, Vector2int.down);

		if(horizontalMatches.Count > 1 || verticalMatches.Count > 1)
		{
			int points = 1;

			if(horizontalMatches.Count > 1)
			{
				points += horizontalMatches.Count;
				horizontalMatches.ForEach(x => RemoveTile(x));
			}

			if(verticalMatches.Count > 1)
			{
				points += verticalMatches.Count;
				verticalMatches.ForEach(x => RemoveTile(x));
			}

			RemoveTile(tile);

			Debug.Log ("Points : "+ points);
		}
	}

	public void Connect(List<TileCandy> list, TileCandy.CandyType type, Vector2int pos, Vector2int dir)
	{
		TileCandy tile = GetTile(pos.x + dir.x, pos.y + dir.y) as TileCandy;

		if(tile != null && tile.type == type)
		{
			list.Add(tile);
			Connect(list, type, pos + dir, dir);
		}
	}

	public bool SwapTiles(Tile tile1, Tile tile2)
	{
		Vector2int pos1 = tile1.pos;
		Vector2int pos2 = tile2.pos;

		//Adjacent check
		float dist = Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);

		if(dist > 1)
			return false;

		world[pos1.x, pos1.y] = tile2;
		world[pos2.x, pos2.y] = tile1;

		tile1.MoveTile(pos2);
		tile2.MoveTile(pos1);

		return true;
	}

	//Move Tile
	public void MoveTile(Tile tile, Vector2int pos)
	{
		world[tile.pos.x, tile.pos.y] = null;

		world[pos.x, pos.y] = tile;

		tile.MoveTile(pos);
	}
	
	//Get Tile From World 
	public Tile GetTile(int x, int y)
	{
		if(x < 0 || x >= WorldSize || y < 0 || y >= WorldSize) return borderTile;
		
		return world[x, y];
	}
	
	public Tile GetTile(Vector2int pos){ return GetTile(pos.x, pos.y); }
	
	//Create Tile
	public bool CreateTile(Vector2int pos, GameObject prefab)
	{
		//Occupied
		if(world[pos.x, pos.y] != null) return false;
		
		//Create tile
		GameObject obj = Instantiate(prefab, pos.ToVector3(), Quaternion.identity) as GameObject;
		Tile newTile = obj.GetComponent<Tile>();
		
		obj.transform.parent = transform;
		obj.name = newTile.GetType().ToString();
		
		tiles.Add(newTile);
		world[pos.x, pos.y] = newTile;
		
		newTile.Initialize();
		
		return true;
	}
	
	//Remove Tile
	public void RemoveTile(Tile tile)
	{
		world[tile.pos.x, tile.pos.y] = null;
		
		tiles.Remove(tile);
		
		tile.Remove();
	}
}
