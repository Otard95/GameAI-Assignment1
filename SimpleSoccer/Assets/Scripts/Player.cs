﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	public bool HasBall { set; get; }

	protected Transform teamBaseTransform;
	protected HumanoidMotor _motor;

	public void SetBasePos(Transform t) {
		teamBaseTransform = t;
	}

	protected void Start () {
		_motor = GetComponent<HumanoidMotor>();
	}
}
