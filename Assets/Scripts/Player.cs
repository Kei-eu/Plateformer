﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float speed;

	Vector2 velocity = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		int horizontal = 0;
		int vertical = 0;

		velocity = Vector2.zero;

        if (Input.GetKey(KeyCode.Q))
		{
			horizontal -= 1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			horizontal += 1;
		}
		if (Input.GetKey(KeyCode.Z))
		{
			vertical += 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vertical -= 1;
		}

		velocity = new Vector2(horizontal * speed, vertical * 5);

		transform.Translate(velocity * Time.deltaTime);

	}
}
