using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour 
{
	public Transform entryPos;
	public Transform exitPos;

	private TileCandy[] content;

	void Start () 
	{
		int length = (int)Mathf.Abs(exitPos.position.x - entryPos.position.x) + (int)Mathf.Abs(exitPos.position.y - entryPos.position.y);

	}

	void Update () 
	{
	
	}
}
