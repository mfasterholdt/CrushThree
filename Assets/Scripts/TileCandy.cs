using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCandy : Tile 
{
	public enum CandyType{ Apple, Banana, Grape, Ruby};
	public CandyType type;
	
	public override void Initialize()
	{
		base.Initialize();

		//Gravity
		force = new Vector2int(0, -1);
	}

	public override void Landing ()
	{
		base.Landing ();

		Level.Instance.Match(this);
	}
}
