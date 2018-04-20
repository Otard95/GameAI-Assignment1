﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour {

	[SerializeField] Player[] players;
	[SerializeField] float playersSortInterval = .5f;
	[SerializeField] GameObject goal;
	[SerializeField] LayerMask opponetLayerMask;

	[Header("UI")]
	[SerializeField] Text scoreText;

	public GameObject Goal { get { return goal; } }
	public LayerMask OpponetLayerMask { get { return opponetLayerMask; } }

	int _goals;
	public int Goals { set { _goals = value; } get { return _goals; } }
	public Team OtherTeam { get; set; }

	private bool _has_ball;
	public bool HasBall { set { _has_ball = value; } get { return _has_ball; } }

	float _last_sort = 0;

	void Start () {

		foreach (Player p in players) {
			// Add events
			foreach (Player otherPlayer in players) {
				if (otherPlayer != p) {
					p.AddCanRecieveListner(otherPlayer.EventHandlerCanRecieve);
				}
			}
		}

	}

	void Update () {

		// Update _has_ball var.
		_has_ball = false;
		foreach (Player p in players) {
			if (p.HasBall) {
				_has_ball = true;
			}
		}

		_last_sort -= Time.deltaTime;

		scoreText.text = Goals.ToString();

	}

	public Player[] GetPlayersByAggretion () {

		if (players.Length == 0) {
			Debug.LogError("Team -> Players array is empty");
			return null;
		}

		if (_last_sort <= 0) { // No point in sorting the list every frame.
			QuickSortPlayers(ref players, 0, players.Length - 1);
			_last_sort = playersSortInterval;
		}

		return players;

	}


	void QuickSortPlayers (ref Player[] arr, int low, int high) {

		int i = low;
		int j = high;

		Player pivot = arr[low + (high - low) / 2];
		float pivotDot = Vector3.Dot(transform.forward, (pivot.transform.position - transform.position).normalized);

		while (i <= j) {

			while (Vector3.Dot(transform.forward, (arr[i].transform.position - transform.position).normalized) > pivotDot) {
				i++;
			}

			while (Vector3.Dot(transform.forward, (arr[j].transform.position - transform.position).normalized) < pivotDot) {
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

	public bool IsPlayerOnTeam (Player player) {
		if (player != null) {
			foreach (Player p in players) {
				if (p == player) {
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public void KickOff () {
		foreach (Player p in players) {
			p.KickOff();
		}
		HasBall = false;
	}

}
