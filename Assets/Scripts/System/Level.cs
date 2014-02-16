using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Level : SingletonComponent<Level> 
{
	[HideInInspector]
	public SelectionObj selection;
	public GameObject selectionPrefab;
	public GameObject selectionTarget;
	
	public Slot trashSlot;

	public Tile borderTile;
	public static Tile BorderTile;

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
	private Tile lastPickedTile;

	private bool matchTransition;
	private float matchTimer;
	private float matchDelay = 0.15f;

	private bool dragging;

	private float nextGlitchTimer;
	
	void Start () 
	{
		nextGlitchTimer = 20f; //time before first glitch

		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
		player = FindObjectOfType<Player>();

		if(selectionPrefab)
		{
			GameObject selectionObj = Instantiate(selectionPrefab) as GameObject;
			selection = selectionObj.GetComponent<SelectionObj>();

			selection.OnMouseClick += OnSelectionClick;
		}

		if(selectionTarget)
		{
			selectionTarget = Instantiate(selectionTarget) as GameObject;
		}

		if(borderTile)
			BorderTile = borderTile;

		RegisterWorld();
	}
	
	void RegisterWorld()
	{
		world = new Tile[WorldSize, WorldSize];

		Tile[] preplaced = GetComponentsInChildren<Tile>();

		for(int i=0, count = preplaced.Length; i<count; i++)
		{
			Tile tile = preplaced[i];
			
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
		List<TileCandy>startBoard = board.CreateBoard();

		//Create Start Glitches
		glitches = new List<TileCandy>();

		startBoard = startBoard.OrderBy(x => Guid.NewGuid()).ToList();

		/*initial glitches
		 * for(int i = 0; i < glitchCount; i++)
		{
			TileCandy newGlitch = startBoard.Find(x => !glitches.Contains(x) && (int)x.type == i);

			if(newGlitch)
				newGlitch.BecomeGlitch();
			else
				break;
		}*/

	}

	void CreateGlitch(TileCandy newGlitch)
	{
		newGlitch.BecomeGlitch();

		if(glitches.Count < glitchCount)
			nextGlitchTimer = UnityEngine.Random.Range(10f, 15f);
	}
	
	void OnSelectionClick(SelectionObj sender, Vector3 pos)
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
		lastPickedTile = pickedTile;

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

			return;
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
			if(tile == lastPickedTile && nextGlitchTimer <= 0)
			{
				CreateGlitch(tile);
				return;
			}

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

			//Add latest match to trash
			if(trashSlot)
			{
				if(trashSlot.currentTile)
				{
					Destroy(trashSlot.currentTile.gameObject);
					trashSlot.currentTile = null;
				}

				GameObject newTrashObj = Instantiate(tile.gameObject, trashSlot.pos.ToVector3(), Quaternion.identity) as GameObject;
				newTrashObj.transform.parent = transform.parent;

				TileCandy newTrashTile = newTrashObj.GetComponent<TileCandy>();
				trashSlot.PlaceTile(newTrashTile);
			}
		}

	}

	public void Connect(List<TileCandy> list, TileCandy.CandyType type, Vector2int pos, Vector2int dir)
	{
		TileCandy tile = GetTile(pos.x + dir.x, pos.y + dir.y) as TileCandy;

		if(tile != null && tile.type == type && !tile.glitch)
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
		player.SetMoveState();

		glitchCount = 0;
		glitches.ForEach(x => x.BecomeNormal());

		/*if(player) return;

		Vector3 spawnPos = board.transform.position + Vector3.right * 11; //p.ToVector3();

		GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.Euler(0, 0, 0)) as GameObject; 


		player = newPlayer.GetComponent<Player>();*/
	}

	void FixedUpdate()
	{
		if(nextGlitchTimer > 0)
			nextGlitchTimer -= Time.deltaTime;

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
		
		return newTile;
	}

	//Place Tile
	public bool PlaceTile(int x, int y, Tile tile)
	{
		Tile currentTile = GetTile(x, y);

		if(currentTile != null) 
			return false;

		tile.targetPos = new Vector3(x, y, 0);
		tile.transform.position = tile.targetPos;
		tile.transform.parent = transform;

		tile.pos.x = x;
		tile.pos.y = y;

		tiles.Add(tile);
		world[x, y] = tile;

		TileCandy tileCandy = tile as TileCandy;

		if(tileCandy)
			tileCandy.SetBoardState();
	
		//Create glitch if missing
		/*if(glitches.Count < glitchCount && !glitches.Exists(g => g.type == tileCandy.type))
			tileCandy.BecomeGlitch();*/

		return true;
	}

	public bool PlaceTile(Vector2int pos, Tile tile){ return PlaceTile(pos.x, pos.y, tile); }

	//Remove Tile
	public void RemoveTile(Tile tile, bool destroy = true)
	{
		world[tile.pos.x, tile.pos.y] = null;

		TileCandy c = tile as TileCandy;

		if(c && c.glitch)
			glitches.Remove(c);

		tiles.Remove(tile);

		if(destroy)
			tile.Remove();
	}
}
