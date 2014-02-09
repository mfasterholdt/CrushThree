using UnityEngine;
using System.Collections;

interface IConnectable
{
	//Connection position
	Vector3 GetConnectionPos();

	//Check if the object is ready to receive
	bool RecieveCheck(TileCandy tile = null);

	//Parse tile to the new object
	bool ParseTile(TileCandy tile);
}

public class WorldObject : MonoBehaviour 
{
}
