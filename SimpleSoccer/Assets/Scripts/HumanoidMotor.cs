﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanoidMotor : MonoBehaviour {

	[SerializeField] float moveForce = 60;
	[SerializeField] float stoppingRadius = 1;	
	[SerializeField] int pursuitSteps = 5;
 
	Vector3 _target_point;
	List<Vector3> _movements;
	Rigidbody _rb;

	void Start () {
		_rb = GetComponent<Rigidbody>();
		_target_point = transform.position;
		_movements = new List<Vector3>();
	}

	void FixedUpdate () {

		Movement();

		Rotate();

	}

	public void Pursuit(GameObject Target)
	{
		Rigidbody rb = Target.GetComponent<Rigidbody>();
		Vector3 newTarget = Target.transform.position + rb.velocity * pursuitSteps;
		Seek(newTarget);
	}

	public void Seek(Vector3 point) {
		_target_point = point;
	}

	public void AddMovement(Vector3 movement) {
		_movements.Add(movement);
	}

	private void Movement() {

		Vector3 move = Vector3.zero;

		foreach (Vector3 m in _movements) {
			move += m;
		}

		Vector3 dir_to_point = _target_point - transform.position;
		move += dir_to_point;
		move = move.magnitude < stoppingRadius ? Vector3.zero : move.normalized; 

		_rb.AddForce(move * moveForce * Time.deltaTime, ForceMode.Force);

		_movements.Clear();
		
	}

	private void Rotate () {

		transform.rotation = Quaternion.LookRotation(_rb.velocity.magnitude < .8f ? transform.forward : _rb.velocity, Vector3.up);

	}

}
