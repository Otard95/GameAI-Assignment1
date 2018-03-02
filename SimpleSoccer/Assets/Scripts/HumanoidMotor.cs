using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanoidMotor : MonoBehaviour {

	[SerializeField] float moveForce = 60;
	[SerializeField] float stoppingRadius = 1;

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

	public void MoveToPoint(Vector3 point) {
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
		move += dir_to_point.magnitude < stoppingRadius ? Vector3.zero : dir_to_point;

		_rb.AddForce(move.normalized * moveForce * Time.deltaTime, ForceMode.Force);

		_movements.Clear();
		
	}

	private void Rotate () {

		transform.rotation = Quaternion.LookRotation(_rb.velocity == Vector3.zero ? transform.forward : _rb.velocity, Vector3.up);

	}

}
