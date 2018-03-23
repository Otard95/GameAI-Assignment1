using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

	[SerializeField] Player[] _players;
	[SerializeField] GameObject goal;

	int _goals;
	public int Goals { private set { _goals = value; } get { return _goals; } }
	public Team OtherTeam { get; set; }

	private bool _has_ball;
	public bool HasBall { private set { _has_ball = value; } get { return _has_ball; } }

	void Start () {
		foreach (Player p in _players) {
			// Add events
			foreach (Player otherPlayer in _players) {
				if (otherPlayer != p) {
					p.AddCanRecieveListner(otherPlayer.EventHandlerCanRecieve);
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

	public Player[] GetPlayersByAggretion () {
		
		Player[] playersOut = new Player[_players.Length];
		Array.Copy(_players, playersOut, _players.Length);

		QuickSortPlayers(ref playersOut, 0, _players.Length-1);

		return playersOut;

	}


	void QuickSortPlayers (ref Player[] arr, int low, int high) {

		int i = low;
		int j = high;

		Player pivot = arr[low + (high - low) / 2];
		float pivotDot = Vector3.Dot(transform.forward, pivot.transform.position - transform.position);

		while (i <= j) {
			
			while (Vector3.Dot(transform.forward, arr[i].transform.position - transform.position) > pivotDot) {
				i++;
			}

			while (Vector3.Dot(transform.forward, arr[j].transform.position - transform.position) < pivotDot) {
				j--;
			}

			if (i <= j) {
				Player tmp = arr[i];
				arr[i] = arr[j];
				arr[j] = tmp;
				i++;
				j--;
			}

		}

		if (j > low) {
			QuickSortPlayers(ref arr, low, j);
		}

		if (i < high) {
			QuickSortPlayers(ref arr, i, high);
		}

	}

}
