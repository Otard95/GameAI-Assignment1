using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerEvent : UnityEvent<GameObject, bool> {
}

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float defaultRightScalar = -10;

	/**
	 * ## Public Fields
	*/

	public PlayerEvent EventCanRecieve;

	/**
	 * ## Protected Filds
	*/

	protected List<GameObject> _can_pass_to;
	protected Transform _team_base_transform;
	protected Team _team;
	protected HumanoidMotor _motor;

	[UsedImplicitly]
	void Awake () {
		if (EventCanRecieve == null) EventCanRecieve = new PlayerEvent();
	}

	protected void Start () {
		_can_pass_to = new List<GameObject>();
		_team_base_transform = transform.parent;
		_team = transform.parent.GetComponent<Team>();
		_motor = GetComponent<HumanoidMotor>();
	}

	public virtual void EventHandlerCanRecieve (GameObject player, bool b) {

		if (b && !_can_pass_to.Contains(player)) {
			_can_pass_to.Add(player);
		} else if (_can_pass_to.Contains(player)) {
			_can_pass_to.Remove(player);
		}

	}

}
