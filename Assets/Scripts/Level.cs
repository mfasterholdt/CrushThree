using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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

	[HideInInspector]
	public List<TileCandy> glitches;
	
	public int glitchCount = 6;	

	public Material glitchMaterial;

	public Board board; 

	public GameObject playerPrefab;
	private Player player;

	private Tile pickedTile;
	
	public delegate void StepEvent();
	public event StepEvent OnStep;

	private bool matchTransition;
	private float matchTimer;
	private float matchDelay = 0.15f;

	private bool dragging;

	void Start () 
	{
		if(selectionPrefab)
		{
			GameObject selectionObj = Instantiate(selectionPrefab) as GameObject;
			selection = selectionObj.GetComponent<Selection>();

			selection.OnMouseClick += OnSelectionClick;
		}

		if(selectionTarget)
		{
			selectionTarget = Instantiate(selectionTarget) as GameObject;
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

		//Initialize boards
		List<TileCandy>startBoard = board.Initialize();

		//Create Start Glitches
		glitches = new List<TileCandy>();

		startBoard = startBoard.OrderBy(x => Guid.NewGuid()).ToList();

		for(int i = 0; i < glitchCount; i++)
		{
			TileCandy newGlitch = startBoard.Find(x => !glitches.Contains(x) && (int)x.type == i);

			if(newGlitch)
			{
				newGlitch.BecomeGlitch();
			}
			else
			{
				break;
			}
		}
	}
	
	void OnSelectionClick(Selection sender, Vector3 pos)
	{	
		//Are we trasitioning?
		if(matchTransition) return;

		Vector2int p = new Vector2int(pos.x, pos.y);

		Tile tile = GetTile(p);

		//Did we hit a tile?
		if(!tile) 
			return;

		//Is a tile already selecte?
		if(!pickedTile)
		{
			Select(tile);
			return;
		}

		//Did we hit the picked tile again?
		if(tile != pickedTile)
		{
			SwapTiles(tile, pickedTile);
		}
	}

	void Select(Tile tile)
	{
		pickedTile = tile;

		selectionTarget.SetActive(true);

		selectionTarget.transform.position = tile.pos.ToVector3();
	}

	void Push(Tile tile, Vector2int force)
	{
		Vector2int p = tile.pos + force;
		
		//***Add collision check
		
		MoveTile(tile, p);
	}

	private List<TileCandy> horizontalMatches = new List<TileCandy>();
	private List<TileCandy> verticalMatches = new List<TileCandy>();
	private List<TileCandy> allMatches = new List<TileCandy>();

	public void Match(TileCandy tile)
	{
		//Glitch Connect
		if(tile.glitch)
		{
			horizontalMatches.Clear();
			verticalMatches.Clear();
			
			ConnectGlitch(horizontalMatches, tile.pos, Vector2int.left);
			ConnectGlitch(horizontalMatches, tile.pos, Vector2int.right);
			
			ConnectGlitch(verticalMatches, tile.pos, Vector2int.up);
			ConnectGlitch(verticalMatches, tile.pos, Vector2int.down);
			
			if(horizontalMatches.Count > 1 || verticalMatches.Count > 1)
			{
				Vector2int spawnPos = tile.pos;

				if(horizontalMatches.Count > 1)
				{
					allMatches.AddRange(horizontalMatches);

					int posX = spawnPos.x;
					allMatches.ForEach(x => posX += x.pos.x);
					spawnPos.x = posX / 3; 
				}

				if(verticalMatches.Count > 1)
				{
					allMatches.AddRange(verticalMatches);
					
					int posY = spawnPos.y;
					allMatches.ForEach(x => posY += x.pos.y);
					spawnPos.y = posY / 3;
				}

				allMatches.Add(tile);

				SpawnPlayer(spawnPos);
			}
		}

		//Regular Connect
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
				allMatches.AddRange(horizontalMatches);
			}

			if(verticalMatches.Count > 1)
			{
				points += verticalMatches.Count;
				allMatches.AddRange(verticalMatches);
			}

			allMatches.Add(tile);
			//Debug.Log ("Points : "+ points);
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

	public void ConnectGlitch(List<TileCandy> list, Vector2int pos, Vector2int dir)
	{
		TileCandy tile = GetTile(pos.x + dir.x, pos.y + dir.y) as TileCandy;
		
		if(tile != null && tile.glitch)
		{
			list.Add(tile);
			ConnectGlitch(list, pos + dir, dir);
		}
	}

	void SpawnPlayer(Vector2int p)
	{
		if(player) return;

		Vector3 spawnPos = p.ToVector3();

		GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.Euler(0, 0, 0)) as GameObject; 

		player = newPlayer.GetComponent<Player>();
	}

	void FixedUpdate()
	{
		CheckDragSwap();

		UpdateMatchTransitions();
	}

	void CheckDragSwap()
	{
		//No tile selected or not dragging
		if(!pickedTile || !selection.dragging) return;

		int x = Mathf.RoundToInt(selection.mousePosition.x);
		int y = Mathf.RoundToInt(selection.mousePosition.y);

		Tile tile = GetTile(x, y);

		if(tile && tile != pickedTile)
		{
			SwapTiles(tile, pickedTile);
		}
	}

	void UpdateMatchTransitions()
	{
		if(allMatches.Count == 0)
		{
			matchTransition = false;
		}
		else
		{
			if(!matchTransition)
			{
				matchTransition = true;
				matchTimer = matchDelay;
			}
			
			float per = matchTimer / matchDelay;
			
			if(per <= 0)
			{
				allMatches.ForEach(x => RemoveTile(x));
				allMatches.Clear();
			}
			else
			{
				allMatches.ForEach(x=> x.MatchTransition(per));
				
				matchTimer -= Time.deltaTime;
			}
		}

	}

	public bool SwapTiles(Tile tile, Tile target)
	{
		Vector2int pos1 = tile.pos;
		Vector2int pos2 = target.pos;

		//Adjacent check
		float dist = Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);

		if(dist > 1)
		{
			Select(tile);
			return false;
		}

		world[pos1.x, pos1.y] = target;
		world[pos2.x, pos2.y] = tile;

		tile.MoveTile(pos2);
		target.MoveTile(pos1);

		selectionTarget.SetActive(false);
		pickedTile = null;

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
	public Tile CreateTile(Vector2int pos, GameObject prefab)
	{
		//Occupied
		if(world[pos.x, pos.y] != null) return null;
		
		//Create tile
		GameObject obj = Instantiate(prefab, pos.ToVector3(), Quaternion.identity) as GameObject;
		Tile newTile = obj.GetComponent<Tile>();
		
		obj.transform.parent = transform;
		obj.name = newTile.GetType().ToString();
		
		tiles.Add(newTile);
		world[pos.x, pos.y] = newTile;
		
		newTile.Initialize();
		
		return newTile;
	}
	
	//Remove Tile
	public void RemoveTile(Tile tile)
	{
		world[tile.pos.x, tile.pos.y] = null;

		TileCandy c = tile as TileCandy;

		if(c && c.glitch)
			glitches.Remove(c);

		tiles.Remove(tile);

		tile.Remove();
	}
}
