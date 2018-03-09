using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	public bool HasBall { set; get; }

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float defaultRightScalar = -10;

	protected Transform teamBaseTransform;
	protected HumanoidMotor _motor;

	public void SetBasePos(Transform t) {
		teamBaseTransform = t;
	}

	protected void Start () {
		_motor = GetComponent<HumanoidMotor>();
	}
}
