﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<GameObject, bool> {
}

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	protected enum States
    {
        Idle,           //Default state. Goes back to start position and wait for kickoff.
        Chase,          //Chase after the ball and try to take it.
        Dribble,         //Move with the ball.
        Recieve,        //Standing by to recieve the ball.
        Support,        //Move to a good position for recieving the ball.
        Kick,           //Shoot at goal.
        Pass            //Pass the ball.
    };


	/**
	 * ## Unity Proporties
	*/

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float offensiveScalarMultiplyer = .7f;
	[SerializeField] protected float defaultRightScalar = -10;
	[SerializeField] protected float fleeRadius = 3;
	[SerializeField] protected float fleeSpeed = 2;

	/**
	 * ## Class Propories
	*/
	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }
	protected bool Stunned;
	protected float stunLimit = 1f;
	protected float stunDuration = 0;

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
	protected States _state;

	/**
	 * ## Components
	*/

	protected Rigidbody _rb;

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
		_state = States.Idle;

		_rb = GetComponent<Rigidbody>();
	}

	protected void DefaultSeek () {
		offenciveScalar = defaultOffenciveScalar + (_game_manager.SoccerBall.transform.position - _team_base_transform.position).x * _team_base_transform.forward.x * offensiveScalarMultiplyer;

		Vector3 targetPosition = _team_base_transform.position + (_team_base_transform.forward * offenciveScalar) + (_team_base_transform.right * rightScalar);
		_motor.Seek(targetPosition);
	}

	protected void SeekDefaultPosition () {
		Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
		_motor.Seek(defaultPos);
	}

	public virtual void EventHandlerCanRecieve (GameObject player, bool b) {

		if (b && !_can_pass_to.Contains(player)) {
			_can_pass_to.Add(player);
		} else if (_can_pass_to.Contains(player)) {
			_can_pass_to.Remove(player);
		}

	}

	void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == _game_manager.SoccerBall.gameObject)
        {
            Ball ball = _game_manager.SoccerBall;
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            ballRigidbody.velocity = Vector3.zero;
            _rb.velocity = Vector3.zero;
            
			if(ball.Owner != null && ball.Owner != this)
			{
				ball.Owner.HasBall = false;
				ball.Owner._team.HasBall = false;
				ball.Owner.Stunned = true;
			}
            HasBall = true;
            ball.Owner = this;
        }
    }
}
