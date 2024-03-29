﻿using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/
	[SerializeField] Team [] teams;
	[SerializeField] Ball ball;
	public Ball SoccerBall { get { return ball; } }
	Vector3 _center;

	/**
	 * ## Class proporties
	*/
	public bool IsKickoff { get; set; }

	#region Singelton

	public static GameManager Instance { get; private set; }

	[UsedImplicitly]
	void Awake () {
		Instance = this;
	}

	#endregion

	// Use this for initialization
	[UsedImplicitly]
	void Start () {
		IsKickoff = false;
		for (int i = 0; i < teams.Length; i++) {
			teams[i].OtherTeam = teams[(i + 1) % teams.Length];
		}
		_center = ball.transform.position;
	}

	public void Goal (GameObject goal) {
		IsKickoff = true;

		for (int i = 0; i < teams.Length; i++) {
			if (teams[i].Goal == goal) {
				teams[(i + 1) % teams.Length].Goals++;
			}
			teams[i].KickOff();
		}
		ball.transform.position = _center;

		StartCoroutine(StartMatch());

	}

	IEnumerator StartMatch () {
		yield return new WaitForSeconds(6);
		ball.transform.position = _center;
		ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
		IsKickoff = false;
	}
}
