using UnityEngine;
using System.Collections;

public class TileBoardCollider : Tile 
{
	public override void Start ()
	{
		base.Start ();

		visuals.SetActive(false);
	}
}
