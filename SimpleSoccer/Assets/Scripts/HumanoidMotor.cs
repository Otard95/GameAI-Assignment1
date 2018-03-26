using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanoidMotor : MonoBehaviour {

	[SerializeField] float moveForce = 60;
	[SerializeField] float stoppingRadius = 1;
	[SerializeField] int pursuitSpeed = 5;

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

	public void Pursuit (GameObject Target) {

		Rigidbody rb = Target.GetComponent<Rigidbody>();
		float T = Vector3.Distance(transform.position, Target.transform.position) / pursuitSpeed;
		Vector3 newTarget = Target.transform.position + rb.velocity * T;

		for (int i = 0; i < 5; i++) {
			T = Vector3.Distance(transform.position, newTarget) / pursuitSpeed;
			newTarget = Target.transform.position + rb.velocity * T;
		}

		Seek(newTarget);

	}

	public void Flee (Vector3 target, float speed) {
		Vector3 desired_velocity = (transform.position - target).normalized * speed;

		AddMovement(desired_velocity);
	}

	public void Seek (Vector3 point) {
		_target_point = point;
	}

	public void Interpose (GameObject targetA, GameObject targetB)
	{

		Vector3 desired_pos = targetA.transform.position + (targetB.transform.position - targetA.transform.position) * .5f;
		Seek(desired_pos);

	}

	public void AddMovement (Vector3 movement) {
		_movements.Add(movement);
	}

	private void Movement () {

		Vector3 move = Vector3.zero;

		foreach (Vector3 m in _movements) {
			move += m;
		}

		Vector3 dir_to_point = _target_point - transform.position;
		move += dir_to_point;
		move = move.magnitude < stoppingRadius ? Vector3.zero : move.normalized;

		_rb.AddForce(move * moveForce, ForceMode.Force);

		_movements.Clear();

	}

	private void Rotate () {

		transform.rotation = Quaternion.LookRotation(_rb.velocity.magnitude < .8f ? transform.forward : _rb.velocity, Vector3.up);

	}

}
