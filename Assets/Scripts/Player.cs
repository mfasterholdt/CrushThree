using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{	
	public float playerMoveSpeed = 5f;
	public float jumpForce = 50f;
	public float gravity = -15f;
	private Vector3 moveDir;

	private bool grounded;

	void Start()
	{

	}
	
	void Update()
	{
		moveDir.x = Input.GetAxis("Horizontal");
		moveDir.y = Input.GetAxis("Vertical");

		if(grounded && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space)))
		{
			rigidbody.AddForce(Vector3.up * jumpForce);
			grounded = false;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		grounded = true;
	}

	void FixedUpdate()
	{
		rigidbody.AddForce(Vector3.up * gravity);

		Vector3 vel = rigidbody.velocity;

		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			vel.x = playerMoveSpeed;
		}
		else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			vel.x = -playerMoveSpeed;
		}
		else
		{
			vel.x = 0;
		}



		rigidbody.velocity = vel;
	}
}
