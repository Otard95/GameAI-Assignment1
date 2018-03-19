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

	new void Start () {
		base.Start();
	}

	// Update is called once per frame
	void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);
	}
}
