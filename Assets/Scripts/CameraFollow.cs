using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float stageSize;
	Player player;
	
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(FindPlayerCoroutine());
		StartCoroutine(FollowPlayerCoroutine());
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

	IEnumerator FollowPlayerCoroutine()
	{
		while (true)
		{
			if (player != null && sat)
				transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
			yield return null;
		}
	}
}
