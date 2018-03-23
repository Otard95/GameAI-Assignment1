using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<GameObject, bool> {
}

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float defaultRightScalar = -10;

	/**
	 * ## Class Propories
	*/
	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }

	/**
	 * ## Public Fields
	*/

	public PlayerEvent EventCanRecieve;

	/**
	 * ## Protected Filds
	*/

	protected GameManager _game_manager;
	protected List<GameObject> _can_pass_to;
	// TODO: Replace with event
	protected bool _is_being_passed_ball;
	protected Transform _team_base_transform;
	protected Team _team;
	protected HumanoidMotor _motor;

	[UsedImplicitly]
	void Awake () {
		if (EventCanRecieve == null) EventCanRecieve = new PlayerEvent();
	}

	protected void Start () {
		_game_manager = GameManager.Instance;
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
