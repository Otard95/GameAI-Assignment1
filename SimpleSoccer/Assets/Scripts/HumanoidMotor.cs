using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanoidMotor : MonoBehaviour {

	[SerializeField] float moveForce = 60;

	Vector3 _target_point;
	List<Vector3> _movements;
	Rigidbody _rb;

	void Start () {
		_rb = GetComponent<Rigidbody>();
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

		move += _target_point - transform.position;

		_rb.AddForce(move.normalized * moveForce * Time.deltaTime, ForceMode.Force);

		_movements.Clear();
		
	}

	private void Rotate () {

		transform.rotation = Quaternion.LookRotation(_rb.velocity, Vector3.up);

	}

}
