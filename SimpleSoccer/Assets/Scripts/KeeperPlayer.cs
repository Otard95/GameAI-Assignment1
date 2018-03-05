using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeeperPlayer : Player {

	[SerializeField] float defaultOffenciveScalar = -25;
	[SerializeField] float defaultRightScalar = 0;

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
