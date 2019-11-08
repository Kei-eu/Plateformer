using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomEnemy : MonoBehaviour
{
	public float circleRadius;
	public LayerMask enemyLayer;
	CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
		Collider2D[] enemiesColliders = Physics2D.OverlapCircleAll(transform.position, circleRadius, enemyLayer);
    }
}
