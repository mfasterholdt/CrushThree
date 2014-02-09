using UnityEngine;
using System.Collections;

public class InternetGate : MonoBehaviour 
{

	public Transform gate;
	public Transform openPos;
	public Transform closedPos;
	public Slot powerSource;

	private string url = "http://www.google.com";
	private float timer;
	private float requestTime = 0.5f;
	private WWW wwwRequest;

	private Vector3 moveTarget;

	void Start () 
	{
		moveTarget = gate.rigidbody2D.transform.position;
	}

	void Update () 
	{
		if(powerSource && powerSource.IsElectric())
			PingCheck();
		else
			moveTarget = closedPos.position;

		Vector3 pos = gate.transform.position;
		pos += (moveTarget - pos) * Time.deltaTime * 3f;
		gate.rigidbody2D.transform.position = pos;
	}

	private void PingCheck()
	{
		if(timer > 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			if(wwwRequest != null && wwwRequest.error == null)
				moveTarget = openPos.position;
			else
				moveTarget = closedPos.position;

			wwwRequest = new WWW (url);
			timer = requestTime;
		}
	}
}
