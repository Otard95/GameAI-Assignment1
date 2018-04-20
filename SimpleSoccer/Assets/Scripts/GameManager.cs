using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/
	[SerializeField] Team [] teams;
	[SerializeField] Ball ball;
	public Ball SoccerBall { get { return ball; } }
	private Vector3 center;

	/**
	 * ## Class proporties
	*/
	public bool IsKickoff { get; set; }

	#region Singelton

	public static GameManager Instance { get; private set; }

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
		center = ball.transform.position;
	}

	public void Goal (GameObject goal) {
		IsKickoff = true;

		for (int i = 0; i < teams.Length; i++) {
			if (teams[i].Goal == goal) {
				teams[(i + 1) % teams.Length].Goals++;
			}
			teams[i].KickOff();
		}
		ball.transform.position = center;

		StartCoroutine(StartMatch());

	}

	IEnumerator StartMatch () {
		yield return new WaitForSeconds(6);
		ball.transform.position = center;
		ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
		IsKickoff = false;
	}
}
