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
	[SerializeField] protected float offensiveScalarMultiplyer = .7f;
	[SerializeField] protected float defaultRightScalar = -10;
	
	/**
	 * ## Class Propories
	*/
	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }

	/**
	 * ## Events
	*/

	protected PlayerEvent _eventCanRecieve;

	public void AddCanRecieveListner (UnityAction<GameObject, bool> action) {
		_eventCanRecieve.AddListener(action);
	}

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
	protected float offenciveScalar;
	protected float rightScalar;

	[UsedImplicitly]
	void Awake () {
		if (_eventCanRecieve == null) _eventCanRecieve = new PlayerEvent();
	}

	protected void Start () {
		_game_manager = GameManager.Instance;
		_can_pass_to = new List<GameObject>();
		_team_base_transform = transform.parent;
		_team = transform.parent.GetComponent<Team>();
		_motor = GetComponent<HumanoidMotor>();

		offenciveScalar = defaultOffenciveScalar;
		rightScalar = defaultRightScalar;
	}

	protected void DefaultSeek () {
		offenciveScalar = defaultOffenciveScalar + (_game_manager.ball.transform.position - _team_base_transform.position).x * _team_base_transform.forward.x * offensiveScalarMultiplyer;

		Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * offenciveScalar) + (_team_base_transform.right * rightScalar);
		_motor.Seek(defaultPos);
	}

	public virtual void EventHandlerCanRecieve (GameObject player, bool b) {

		if (b && !_can_pass_to.Contains(player)) {
			_can_pass_to.Add(player);
		} else if (_can_pass_to.Contains(player)) {
			_can_pass_to.Remove(player);
		}

	}

}
