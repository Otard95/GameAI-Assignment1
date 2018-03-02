using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanoidMotor : MonoBehaviour {



	List<Force> _forces;
	Rigidbody _rb;

	void Start () {
		_rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		Vector3 force = Vector3.zero;
		Vector3 impulse = Vector3.zero;

		foreach (Force f in _forces) {
			if (f.mode == ForceMode.Force)
				force += f.dir;
			else if (f.mode == ForceMode.Impulse)
				impulse += f.dir;
		}

		_rb.AddForce(force * Time.deltaTime, ForceMode.Force);
		_rb.AddForce(impulse * Time.deltaTime, ForceMode.Impulse);

		_forces.Clear();

	}

	public void MoveToPoint(Vector3 point) {

	}

	public void AddForce(Vector3 force, ForceMode mode) {
		_forces.Add(new Force(force, mode));
	}

}

struct Force {
	public Vector3 dir;
	public ForceMode mode;
	public Force(Vector3 _force, ForceMode _mode) {
		dir = _force;
		mode = _mode;
	}
}
