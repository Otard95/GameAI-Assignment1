using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

	[SerializeField] Player[] _players;

	private int _goals;
	public int Goals { private set { _goals = value; } get { return _goals; } }

	private bool _has_ball;
	public bool HasBall { private set { _has_ball = value; } get { return _has_ball; } }

	void Start () {
		foreach (Player p in _players) {
			p.SetBasePos(transform);
			// Add events
			foreach (Player otherPlayer in _players) {
				if (otherPlayer != p) {
					p.CanRecieve.AddListener(otherPlayer.EventCanRecieve);
				}
			}
		}
	}

	void Update () {

		// Update _has_ball var.
		_has_ball = false;
		foreach (Player p in _players) {
			if (p.HasBall) {
				HasBall = true;
			}
		}

	}

}
