﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/
	[SerializeField] Team [] teams;
	[SerializeField] GameObject ball;
	public GameObject Ball { get { return ball; } }

	/**
	 * ## Class proporties
	*/
	public bool IsKickoff { get; private set; }

	#region Singelton

	public static GameManager Instance { get; set; }

	void Awake () {
		Instance = this;
	}

#endregion

	// Use this for initialization
	[UsedImplicitly]
	void Start () {
		IsKickoff = true;
		for (int i = 0; i < teams.Length; i++)
		{
			teams[i].OtherTeam = teams[(i + 1) % teams.Length];
		}
	}

	void Goal (int teamID) {
		/* Possible implementation
		_isKickOff = true;
		teams[teamID].Score++;

		for (int i = 0; i < teams.Length; i++)
		{
			teams[i].KickOff();
		}
		*/
	}
	
}
