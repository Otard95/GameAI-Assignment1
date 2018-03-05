using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

	[SerializeField] Player[] _players;

	// Use this for initialization
	void Start () {
		foreach (Player p in _players) {
			p.SetBasePos(transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
