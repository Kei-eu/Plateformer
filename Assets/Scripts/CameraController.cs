using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	Player player;
	Camera cam;
	Scene scene;

	float height;
	float width;

	float minXLimit;
	float maxXLimit;
	float minYLimit;
	float maxYLimit;

	float initialOrthographicSize = 10;

	// Start is called before the first frame update
	void Start()
	{
		cam = GetComponent<Camera>();
		scene = FindObjectOfType<Scene>();

		height = cam.orthographicSize * 2;
		width = height * cam.aspect;

		minXLimit = scene.BottomLeftLimit.position.x + width / 2;
		maxXLimit = scene.TopRightLimit.position.x - width / 2;
		minYLimit = scene.BottomLeftLimit.position.y + height / 2;
		maxYLimit = scene.TopRightLimit.position.y - height / 2;

		StartCoroutine(FindPlayerCoroutine());
	}
	private void LateUpdate()
	{
		if (player != null)
			MoveToPlayer();
	}

	IEnumerator FindPlayerCoroutine()
	{
		while (true)
		{
			if (player == null)
				player = FindObjectOfType<Player>(); // FindObjectOfType take a lot of ressources
			yield return null;
		}
	}

	void MoveToPlayer()
	{
		Vector3 target = new Vector3(Mathf.Clamp(player.transform.position.x, minXLimit, maxXLimit), Mathf.Clamp(player.transform.position.y, minYLimit, maxYLimit), transform.position.z);
		transform.position = target;
	}

	Coroutine zoomCoroutine;
	public void Zoom(float duration, float magnitude = 1)
	{
		if (zoomCoroutine == null)
			zoomCoroutine = StartCoroutine(ZoomCoroutine(magnitude, duration));
	}

	IEnumerator ZoomCoroutine(float magnitude, float duration)
	{
		cam.orthographicSize = initialOrthographicSize / magnitude;

		yield return null;

		zoomCoroutine = null;
	}
}
