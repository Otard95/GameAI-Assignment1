using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanoidMotor))]
public class TestMotor : MonoBehaviour {

	HumanoidMotor motor;

	// Use this for initialization
	void Start () {
		motor = GetComponent<HumanoidMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
			motor.AddMovement(new Vector3(8, 0, 3));
		}

		if (Input.GetMouseButtonDown(0)) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hit = Physics.RaycastAll(ray);

			if (hit.Length > 0) {

				for (int i = 0; i < hit.Length; i++) {
					var h = hit[i];

					if (h.collider.CompareTag("Pitch")) {
						motor.MoveToPoint(h.point);
					}
				}

			}
		}
	}
}
