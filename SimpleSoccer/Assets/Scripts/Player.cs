using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<GameObject, bool> {
}

[RequireComponent(typeof(HumanoidMotor))]
public abstract class Player : MonoBehaviour {

	/**
	 * ## Unity Proporties
	*/

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float offensiveScalarMultiplyer = .7f;
	[SerializeField] protected float defaultRightScalar = -10;
	[SerializeField] protected float fleeRadius = 3;
	[SerializeField] protected float fleeSpeed = 2;
	[SerializeField] protected float ballPassSpeed = 15;
	[SerializeField] protected int ballPassSteps = 5;
	[SerializeField] protected float minOpponentDistForPass = 2;

	/**
	 * ## Class Propories
	*/
	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }
	public bool IsBeingPassedBall { get; set; }

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
	protected Transform _team_base_transform;
	protected Team _team;
	protected HumanoidMotor _motor;
	protected float offenciveScalar;
	protected float rightScalar;
	protected bool Stunned;
	protected float stunLimit = 1f;
	protected float stunDuration = 0;

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

	protected void KickBall (Vector3 dir, float force = -1) {
		if (force == -1) force = ballPassSpeed;
		Ball ball = _game_manager.SoccerBall;
		Rigidbody ball_rb = ball.GetComponent<Rigidbody>();
		ball_rb.velocity = Vector3.zero; // stop the balls velocity so it doesn't effect the pass	
		ball_rb.AddForce(dir.normalized * force, ForceMode.Impulse);
		ball.Owner = null;
	}

	public virtual void EventHandlerCanRecieve (GameObject player, bool b) {

		if (b && !_can_pass_to.Contains(player)) {
			_can_pass_to.Add(player);
		} else if (_can_pass_to.Contains(player)) {
			_can_pass_to.Remove(player);
		}

	}

	void OnCollisionEnter (Collision collision) {
		if (collision.collider.CompareTag("Ball")) {
			Ball ball = _game_manager.SoccerBall;
			Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
			ballRigidbody.velocity = _rb.velocity;
			ballRigidbody.angularVelocity = Vector3.zero;

			if (ball.Owner == null) {

				HasBall = true;
				ball.Owner = this;
			} else if (!_team.IsPlayerOnTeam(ball.Owner) && !Stunned) {
				ball.Owner.HasBall = false;
				ball.Owner._team.HasBall = false;
				ball.Owner.ApplyStun();
				HasBall = true;
				ball.Owner = this;
			}
		}
	}

	/**
	 * ## Abstract functions
	*/

	public abstract void KickOff ();
	public abstract void ApplyStun ();
}
