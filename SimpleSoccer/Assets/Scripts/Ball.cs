﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	public Player Owner;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "Blue Goal" || other.gameObject.name == "Red Goal")
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			Owner = null;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			GameManager.Instance.Goal(other.gameObject);
		}
	}
}
