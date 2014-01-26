using UnityEngine;
using System.Collections;

interface IConnectable
{
	//When true the connector takes control
	bool RecieveCheck(TileCandy tile);
}

public class WorldObject : MonoBehaviour 
{
}
