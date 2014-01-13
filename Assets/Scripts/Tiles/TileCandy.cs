using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCandy : Tile 
{
	public enum CandyType{ Apple, Banana, Grape, Ruby, Emerald, Diamond};

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

	public void MatchTransition(float per)
	{
		visuals.transform.localScale = Vector3.one * per;
	}
}
