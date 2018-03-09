using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }

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
