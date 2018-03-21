using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/
	[SerializeField] Team [] teams;
	[SerializeField] GameObject [] goals;
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

		if(teams.Length != 2)
		{
			Debug.LogError("Team array invalid");
		}
		else
		{
			teams[0].TeamId = false; 	//ID 0
			teams[1].TeamId = true;		//ID 1
		}
	}

	// Update is called once per frame
	[UsedImplicitly]
	void Update () {

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

	public GameObject GetGoal(bool id)
	{
		if(id)
		{
			return goals[1];
		}
		else
		{
			return goals[0];
		}
	}
}
