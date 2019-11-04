using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	public int horizontalRayCount;
	public int verticalRayCount;
	public LayerMask layerObstacle;

	BoxCollider2D boxCollider;
	Vector2 bottomLeft;
	Vector2 bottomRight;
	Vector2 topLeft;
	Vector2 topRight;

	float verticalRaySpacing;

	// Start is called before the first frame update
	void Start()
    {
		boxCollider = GetComponent<BoxCollider2D>();
		verticalRaySpacing = boxCollider.bounds.size.y / (verticalRayCount - 1);
	}


	public void Move(Vector2 velocity)
	{
		CalculateBounds();
		HorizontalMove(ref velocity);
		transform.Translate(velocity);
	}

	void CalculateBounds()
	{
		bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
		bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
		topLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y);
		topRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y);
	}
	
	void HorizontalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x);

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;
			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);

			Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(direction, 0), distance, layerObstacle);

			if (hit)
			{
				velocity.x = hit.distance * direction;
			}
		}
	}
}
