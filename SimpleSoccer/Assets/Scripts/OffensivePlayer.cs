﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensivePlayer : Player {

	[SerializeField] float defaultOffenciveScalar = -5;
	[SerializeField] float defaultRightScalar = -10;

	new void Start () {
		base.Start();
	}

	// Update is called once per frame
	void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);

	}
}