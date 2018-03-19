using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private bool _isKickOff;
	private GameManager _instance;

	[SerializeField] Team [] teams;
	[SerializeField] Transform ball;

	public GameManager Instance
	{
		get { return _instance;  }
	}

	void Awake()
	{
		_instance = this;
	}

	// Use this for initialization
	void Start ()
	{ 
		_isKickOff = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Goal(int teamID)
	{
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
