﻿using System.Collections;
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

		Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);
	}
}
