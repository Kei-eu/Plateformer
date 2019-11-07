//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
//public class MovementController : MonoBehaviour
//{
//	public int horizontalRayCount;
//	public int verticalRayCount;
//	public LayerMask layerObstacle;

//	BoxCollider2D boxCollider;
//	Vector2 bottomLeft;
//	Vector2 bottomRight;
//	Vector2 topLeft;
//	Vector2 topRight;

//	float verticalRaySpacing;

//	// Start is called before the first frame update
//	void Start()
//    {
//		boxCollider = GetComponent<BoxCollider2D>();
//		verticalRaySpacing = boxCollider.bounds.size.y / (verticalRayCount - 1);
//	}


//	public void Move(Vector2 velocity)
//	{
//		CalculateBounds();
//		HorizontalMove(ref velocity);
//		transform.Translate(velocity);
//	}

//	void CalculateBounds()
//	{
//		bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
//		bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
//		topLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y);
//		topRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y);
//	}

//	void HorizontalMove(ref Vector2 velocity)
//	{
//		float direction = Mathf.Sign(velocity.x);
//		float distance = Mathf.Abs(velocity.x);

//		for (int i = 0; i < verticalRayCount; i++)
//		{
//			Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;
//			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);

//			Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
//			RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(direction, 0), distance, layerObstacle);

//			if (hit)
//			{
//				velocity.x = hit.distance * direction;
//			}
//		}
//	}
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	public int horizontalRayCount;
	public int verticalRayCount;
	public LayerMask layerObstacle;
	//public LayerMask layerOneWayPlateform;
	public Collisions collisions;

	float skinWidth;
	float pitDistance;

	BoxCollider2D boxCollider;
	Vector2 bottomLeft, bottomRight, topLeft, topRight;

	float verticalRaySpacing;
	float horizontalRaySpacing;

	public struct Collisions
	{
		public bool top, bottom, left, right;
		public bool frontPit;

		public void Reset()
		{
			top = bottom = left = right = false;
			frontPit = false;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		skinWidth = 1 / 16f;
		CalculateRaySpacings();
		pitDistance = 0.5f;
	}

	public void Move(Vector2 velocity)
	{
		collisions.Reset();

		CalculateBounds();
		if (velocity.x != 0)
			HorizontalMove(ref velocity);
		if (velocity.y != 0)
			VerticalMove(ref velocity);

		DetectFrontPit(velocity);

		transform.Translate(velocity);
	}

	void HorizontalMove(ref Vector2 velocity)
	{
		// XXX brique sort du mur, reassign valeur de distance dans le boucle

		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? bottomRight : bottomLeft;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(0, verticalRaySpacing * i);

			//Debug.DrawLine(origin, origin + new Vector2(direction * 1, 0));
			//Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(direction, 0),
				distance,
				layerObstacle
				);

			if (hit)
			{
				if (!(hit.transform.gameObject.tag == "oneWayPlatform"))
				{
					velocity.x = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if (direction < 0)
						collisions.left = true;
					else if (direction > 0)
						collisions.right = true;
				}
			}
		}
	}

	void VerticalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y) + skinWidth;

		Vector2 baseOrigin = direction == 1 ? topLeft : bottomLeft;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(horizontalRaySpacing * i, 0);

			Debug.DrawLine(origin, origin + new Vector2(0, direction * distance));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(0, direction),
				distance,
				layerObstacle
				);

			if (hit)
			{
				//if (!(layerOneWayPlateform == (layerOneWayPlateform | (1 << hit.transform.gameObject.layer)) && direction > 0)) // Test if not touching OneWayPlateform from bottom to top
				if(!(hit.transform.gameObject.tag == "oneWayPlatform" && direction > 0))
				{
					//Block the plateform
					velocity.y = (hit.distance - skinWidth) * direction;
					distance = hit.distance - skinWidth;

					if (direction < 0)
						collisions.bottom = true;
					else if (direction > 0)
						collisions.top = true;
				}
			}
		}
	}

	void CalculateRaySpacings()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);

		verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
		horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
	}

	void CalculateBounds()
	{
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2f);

		bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		topLeft = new Vector2(bounds.min.x, bounds.max.y);
		topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void DetectFrontPit(Vector2 velocity)
	{
		// Offset of origin to go to the last pixel of the plateform before turning
		bottomRight.x += -0.3f;
		bottomLeft.x += +0.3f;


		Vector2 origin = velocity.x > 0 ? bottomRight : bottomLeft;
		
		Debug.DrawLine(origin, origin + Vector2.down, Color.red);
		RaycastHit2D hit = Physics2D.Raycast(
			origin,
			Vector2.down,
			pitDistance,
			layerObstacle
			);

		if (!hit)
		{
			collisions.frontPit = true;
		}
		
	}
}

