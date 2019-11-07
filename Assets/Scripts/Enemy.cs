using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
	public float speed;
	public float stopTimeFlip;
	public bool facingRight;

	Animator anim;
	MovementController movementController;
	Vector2 velocity = new Vector2();
	SpriteRenderer spriteRenderer;
	Coroutine flipCoroutine;


    // Start is called before the first frame update
    void Start()
    {
		movementController = GetComponent<MovementController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();

		velocity.x = speed;

		StartFacing();
	}
	void StartFacing()
	{
		if (facingRight)
		{
			velocity.x = speed;
		}
		else
		{
			velocity.x = -speed;
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
	}

	private void Update()
	{
		UpdateMove();
		UpdateFlip();
	}


	void UpdateMove()
    {
		movementController.Move(velocity * Time.deltaTime);
    }

	void UpdateFlip()
	{
		if ((velocity.x > 0 && movementController.collisions.right) || (velocity.x < 0 && movementController.collisions.left))
		{
			Flip();
		}
		else if (movementController.collisions.frontPit)
		{
			Flip();
		}
	}

	void Flip()
	{
		if (flipCoroutine == null)
		{
			flipCoroutine = StartCoroutine(FlipCoroutine());
		}
	}

	IEnumerator FlipCoroutine()
	{
		float actualVelocity = velocity.x;
		velocity.x = 0;

		anim.Play("Idle");

		yield return new WaitForSeconds(stopTimeFlip);

		anim.Play("Run");

		spriteRenderer.flipX = !spriteRenderer.flipX;
		velocity.x = actualVelocity * -1; // Invert direction
		flipCoroutine = null;
	}
}
