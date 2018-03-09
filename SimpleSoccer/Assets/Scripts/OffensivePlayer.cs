using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum States
{
	idle,
	chaseBall,
	drible,
	recieveBall,
	support,
	returnHome,
	kickBall
};

public class OffensivePlayer : Player {

	[SerializeField] float defaultOffenciveScalar = -5;
	[SerializeField] float defaultRightScalar = -10;

	HumanoidMotor _motor;

	void Start () {
		_motor = GetComponent<HumanoidMotor>();
	}

	// Update is called once per frame
	public override void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);
	}
}
