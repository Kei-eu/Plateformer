//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Player : MonoBehaviour
//{
//	[Tooltip("Number of meters by seconds")]
//	public float speed;

//	Vector2 velocity = new Vector2();

//	MovementController movementController;

//    // Start is called before the first frame update
//    void Start()
//    {
//		movementController = GetComponent<MovementController>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//		int horizontal = 0;
//		int vertical = 0;

//		velocity = Vector2.zero;

//        if (Input.GetKey(KeyCode.Q))
//		{
//			horizontal -= 1;
//		}
//		if (Input.GetKey(KeyCode.D))
//		{
//			horizontal += 1;
//		}
//		if (Input.GetKey(KeyCode.Z))
//		{
//			vertical += 1;
//		}
//		if (Input.GetKey(KeyCode.S))
//		{
//			vertical -= 1;
//		}

//		velocity = new Vector2(horizontal * speed, vertical * speed);

//		movementController.Move(velocity * Time.deltaTime);

//	}
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	public float acceleration;
	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	float minSpeedThreshold;
	float maxFallingSpeed;

	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;

	public float airControl;
	
	float gravity;
	float jumpForce;

	Vector2 velocity = new Vector2();
	MovementController movementController;

	// Start is called before the first frame update
	void Start()
	{
		acceleration = acceleration * 5; // reduce big numbers for acceleration in Unity insperctor
		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
		movementController = GetComponent<MovementController>();

		// Math calculation for gravity and jumpForce
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJump, 2);
		jumpForce = Mathf.Abs(gravity) * timeToMaxJump;
		maxFallingSpeed = -jumpForce;
	}

	// Update is called once per frame
	void Update()
	{
		int horizontal = 0;

		if (movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;
		
		horizontal = 0;

		if (Input.GetKey(KeyCode.D))
		{
			horizontal += 1;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			horizontal -= 1;
		}

		if (Input.GetKeyDown(KeyCode.Space) && movementController.collisions.bottom)
		{
			Jump();
		}

		float ControlModifier = 1f;
		if (!movementController.collisions.bottom) // Not on the ground
		{
			ControlModifier = airControl;
		}

		velocity.x += horizontal * acceleration * ControlModifier * Time.deltaTime;

		if (velocity.x > maxSpeed)
			velocity.x = maxSpeed;
		if (velocity.x < -maxSpeed)
			velocity.x = -maxSpeed;

		if (horizontal == 0)
		{
			if (velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * Time.deltaTime;
			else if (velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}
		if (horizontal == 0 && velocity.x > 0)
			velocity.x -= acceleration * Time.deltaTime;
		else if (horizontal == 0 && velocity.x < 0)
			velocity.x += acceleration * Time.deltaTime;


		velocity.y += gravity * Time.deltaTime;

		if (velocity.y < maxFallingSpeed)
			velocity.y = maxFallingSpeed;



		movementController.Move(velocity * Time.deltaTime);
	}

	void Jump()
	{
		velocity.y = jumpForce;
	}
}

