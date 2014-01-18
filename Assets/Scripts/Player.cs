using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{	
	public float playerMoveSpeed = 5f;
	public float climbSpeed = 5f;
	public float jumpForce = 50f;
	public float gravity = -15f;

	public Vector3 cameraOffset;
	private Camera cam;
	
	private Vector3 moveDir;

	private bool grounded;
	private float vel;

	private bool camFollow;
	private float cameraEaseIn;
	private float wallCollision;
	
	List<Trigger> triggers = new List<Trigger>();

	//****Create STATES to handle all the stuffs!!!

	void Start()
	{
		cam = Camera.main;
	}
	
	void Update()
	{
		moveDir.x = Input.GetAxis("Horizontal");
		moveDir.y = Input.GetAxis("Vertical");

		if(grounded && Input.GetKey(KeyCode.Space))
		{
			rigidbody.AddForce(Vector3.up * jumpForce);
			grounded = false;
		}
	}

	void FixedUpdate()
	{		
		//Movement
		Vector3 nextPos = rigidbody.transform.position;
		float inputX = Input.GetAxis("Horizontal");

		if(wallCollision == 0 || Mathf.Sign(inputX) == wallCollision)		
		{
			nextPos.x += inputX * playerMoveSpeed * Time.deltaTime;
		}

		float inputY = Input.GetAxis("Vertical");

		if(triggers.Count > 0)
		{
			//Climbing
			if(Mathf.Abs(inputY) > 0.3f)
			{
				nextPos.y += inputY * climbSpeed * Time.deltaTime;
			}
		}
		else
		{
			//Gravity
			rigidbody.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
		}


		rigidbody.MovePosition(nextPos);

		//Camera Follow
		if(nextPos.x != rigidbody.transform.position.x)
		{
			cameraEaseIn += Time.deltaTime * 0.3f;
			cameraEaseIn = Mathf.Min (cameraEaseIn, 1);
		}

		Vector3 nextCamPos = cam.transform.position;

		Vector3 camTarget = nextPos - transform.forward * cameraOffset.x + transform.up * cameraOffset.y;
		nextCamPos += ((camTarget - nextCamPos) * Time.deltaTime * 4.25f) * cameraEaseIn;

		cam.transform.position = nextCamPos;
	}
	
	void OnCollisionEnter(Collision col)
	{
		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			ContactPoint c = col.contacts[i];
			
			float dot = Vector3.Dot(c.normal, Vector3.up); 
			
			if(dot > 0.6f)
				grounded = true;
			
			float dotWall = Vector3.Dot(c.normal, Vector3.right); 
			
			if(Mathf.Abs(dotWall) > 0.5f)
				wallCollision = Mathf.Sign(dotWall);
		}
	}
	
	void OnCollisionExit(Collision col)
	{
		for(int i = 0, count = col.contacts.Length; i < count; i++)
		{
			ContactPoint c = col.contacts[i];
			
			float dotWall = Vector3.Dot(c.normal, Vector3.right); 
			
			if(Mathf.Abs(dotWall) > 0.5f)
				wallCollision = 0;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		Trigger t = col.GetComponent<Trigger>();

		if(t && !triggers.Contains(t))
			triggers.Add(t);
	}

	void OnTriggerExit(Collider col)
	{
		Trigger t = col.GetComponent<Trigger>();
		
		if(t && triggers.Contains(t))
			triggers.Remove(t);
	}

}