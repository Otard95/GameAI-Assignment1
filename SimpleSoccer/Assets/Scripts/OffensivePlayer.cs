using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
