using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	protected Transform teamBaseTransform;

	public void SetBasePos(Transform t) {
		teamBaseTransform = t;
	}

	void Start () {
		
	}

	public virtual void Update () {

	}
}
