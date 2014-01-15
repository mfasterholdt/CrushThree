using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{	
	public float playerMoveSpeed = 5f;
	public float jumpForce = 50f;
	public float gravity = -15f;

	public Vector3 cameraOffset;
	private Camera cam;

	private BezierCurveManager levelCurve;
	private Vector3 moveDir;

	private bool grounded;
	private float levelPos = 1.7f;

	void Start()
	{
		levelCurve = FindObjectOfType<BezierCurveManager>();

		cam = Camera.main;

		previousPos = transform.position;
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

	float vel;

	Vector3 previousPos;

	void FixedUpdate()
	{		

		//Movement
		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			vel = -playerMoveSpeed;
		}
		else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			vel = playerMoveSpeed;
		}
		else
		{
			vel = 0;
		}

		levelPos += vel * Time.deltaTime;

		Vector3 curvePos = levelCurve.GetPositionAtTime(levelPos);

		curvePos.y = transform.position.y;

		rigidbody.MovePosition(curvePos);

		//Gravity
		rigidbody.AddForce(Vector3.up * gravity);


		//Look at
		if(vel != 0)
		{
			Vector3 lookDir = previousPos - transform.position;

			lookDir.y = 0;

			transform.LookAt(transform.position + lookDir * Mathf.Sign(vel), Vector3.up);
		}
		
		previousPos = transform.position;

		//Camera Position
		Vector3 nextCamPos = cam.transform.position;
		Vector3 camTarget = transform.position + transform.right * cameraOffset.x + transform.up * cameraOffset.y;
		nextCamPos += (camTarget - nextCamPos) * Time.deltaTime * 8f;
		cam.transform.position = nextCamPos;

		//Camera Rotation
		Vector3 lookAtTarget = transform.position - cam.transform.position;
		lookAtTarget += cam.transform.position;
		lookAtTarget.y = cam.transform.position.y;
		//lookAtTarget.Normalize();

		Vector3 nextLookAt = cam.transform.position + cam.transform.forward; 
		nextLookAt += (lookAtTarget - nextLookAt) * Time.deltaTime * 8f;

		cam.transform.LookAt(nextLookAt, Vector3.up);
	}
}
