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

	int jumpCount;
	public int maxAirJump;
	[Range(0,1)]
	public float airControl;
	
	float gravity;
	float jumpForce;
	Animator anim;
	bool doubleJump;
	bool freeze;

	SpriteRenderer SR;
	AnimationTimes animationTimes;

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

		anim = GetComponent<Animator>();
		SR = GetComponent<SpriteRenderer>();
		animationTimes = GetComponent<AnimationTimes>();
	}

	// Update is called once per frame
	void Update()
	{
		int horizontal = 0;

		if (movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;

		if (Input.GetKey(KeyCode.D) && !freeze)
		{
			horizontal += 1;
		}
		if (Input.GetKey(KeyCode.Q) && !freeze)
		{
			horizontal -= 1;
		}

		UpdateFlip(velocity);

		UpdateJump();

		UpdateAnimations();

		float ControlModifier = 1f;
		if (!movementController.collisions.bottom) // Not on the ground
		{
			ControlModifier = airControl;
		}
		velocity.x += horizontal * acceleration * ControlModifier * Time.deltaTime;

		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal; // horizontal = 1 or -1;

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
		if (!movementController.collisions.bottom)
		{
			doubleJump = true;
			if (jumpCount == 0)
				jumpCount++;
		}

		jumpCount++;
		velocity.y = jumpForce;
	}

	void UpdateJump()
	{
		if (movementController.collisions.bottom || movementController.collisions.left || movementController.collisions.right)
		{
			jumpCount = 0;
			doubleJump = false;
		}

		if (Input.GetKeyDown(KeyCode.Space) && jumpCount <= maxAirJump)
		{
			Jump();
		}
	}

	void UpdateAnimations()
	{
		if (freeze)
			return;

		// On the ground
		if (movementController.collisions.bottom)
		{
			if(velocity.x == 0)
				anim.Play("VGIdle");
			else if (velocity.x != 0)
				anim.Play("VGRun");
		}
		// In air
		else
		{
			if(doubleJump)
				anim.Play("VGDJump");
			else if (velocity.y > 0)
			{
				if (movementController.collisions.left || movementController.collisions.right)
				{
					anim.Play("VGWJump");
				}
				else anim.Play("VGJump");
			}
				
			else if (velocity.y < 0)
				anim.Play("VGFall");
		}
	}

	void UpdateFlip(Vector2 velocity)
	{
		if(velocity.x > 0)
		{
			SR.flipX = false;
		}
		if (velocity.x < 0)
		{
			SR.flipX = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();

		if (enemy != null)
			HitEnemy();

	}

	Coroutine hitEnemy;
	void HitEnemy()
	{
		if (hitEnemy == null)
		{
			hitEnemy = StartCoroutine(HitEnemyCoroutine());
		}
	}

	IEnumerator HitEnemyCoroutine()
	{
		anim.Play("VGHit");
		freeze = true;

		yield return new WaitForSeconds(animationTimes.GetTime("VGHit"));

		// Destroy and respawn Player
		SpawnPlayer spawnPlayer = FindObjectOfType<SpawnPlayer>();
		spawnPlayer.Spawn();
		Destroy(gameObject);
	}
}

