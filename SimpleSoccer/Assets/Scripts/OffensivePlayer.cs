using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class OffensivePlayer : Player {
	enum States {
		Idle,       // The player is idle and has no goal
		Chase,      // The team does not have the ball, go get it.
		Dribble,    // Player had the ball and is trying to get to an advantages position for a pass.
		Support,    // Player's team has the ball. The player advances up the pitch with the team, but stays further back to defend.
		Receive,    // The player is being passed the ball, and activly tries to catch it.
		Pass,       // The player has the ball and is trying to pass it to another player(offensive)
		Kick,       // The player is in a position to score a goal, so he goes for it.
		Stunned     // Just lost the ball. Need some time to figure things out.
	}

	/**
 * ## Unity Proporties
*/

	[SerializeField] float sphereCastRadius = 1.5f;
	[SerializeField] float _kickForce = 15;

	/**
 * ## Private Fields
*/
	States _state;

	[UsedImplicitly]
	new void Start () {
		base.Start();
		_state = States.Idle;
	}

	// Update is called once per frame
	[UsedImplicitly]
	void Update () {
		if (_has_ball) {
			if (_game_manager.SoccerBall != null) {
				float distanceToBall = Vector3.Distance(transform.position, _game_manager.SoccerBall.transform.position);

				if (distanceToBall > 10) {
					_has_ball = false;
				}
			}
		}

		#region State Transitions Switch

		switch (_state) {
			case States.Idle: {
					Idle();
					IdleTransitions();
					break;
				}
			case States.Stunned: {
					StunTick();
					StunnedTransitions();
					break;
				}
			case States.Chase: {
					Chase();
					ChaseTransitions();
					break;
				}
			case States.Dribble: {
					Dribble();
					DribbleTransitions();
					break;
				}
			case States.Kick: {
					KickAction();
					KickTransitions();
					break;
				}
			case States.Receive: {
					Receive();
					ReceiveTransitions();
					break;
				}
			case States.Support: {
					Support();
					SupportTransition();
					break;
				}
			case States.Pass: {
					Pass();
					PassTransition();
					break;
				}
		}
		#endregion
	}

	#region State Transitions
	void IdleTransitions () {
		if (!_game_manager.IsKickoff) {
			_state = States.Chase;
		}
	}

	void DribbleTransitions () {
		// if player lost the ball, go chase
		if (!HasBall) {
			_state = States.Chase;
		}
		// if opponent is too close try doing a pass
		/* else if (Physics.OverlapSphere(transform.position, minOpponentDistForPass, _team.OpponetLayerMask).Length != 0 || 
		_rb.velocity.magnitude < 1) {
	_state = States.Pass;
} */
		// in a position to score, shoot.
		else if ((_team.OtherTeam.Goal.transform.position - transform.position).magnitude < 10) {
			_state = States.Kick;
		}
	}

	void ChaseTransitions () {
		// if player got the ball go dribble
		if (HasBall) {
			_state = States.Dribble;
		}
		// if team got the ball go support
		else if (_team.HasBall) {
			_state = States.Support;
		}
	}

	void ReceiveTransitions () {
		if (HasBall) {
			_state = States.Dribble;
		} 
		else if (!_team.HasBall){
			_state = States.Chase;
			_has_ball = false;
		}
	}

	void SupportTransition () {
		// if team lost the ball go chase
		if (!_team.HasBall) {
			_state = States.Chase;
		}
		// if player got the ball go dribble
		else if (HasBall) {
			_state = States.Dribble;
		}
		// if player is being passed the ball go receive
		else if (IsBeingPassedBall) {
			_state = States.Receive;
		}
	}

	void StunnedTransitions () {
		//if no longer disoriented go chase again
		if (!Stunned) {
			_state = States.Chase;
		}
	}

	void KickTransitions () {
		// no longer in possession of ball
		if (!HasBall) {
			// if the team still have it go support
			if (_team.HasBall) {
				_state = States.Support;
			}
			// if the team no longer has it go chase
			else {
				_state = States.Chase;
			}
		}
	}

	void PassTransition () {
		// player goes to Support state when ball is passed
		if (!HasBall) _state = States.Support;
	}

	#endregion

	#region State Behaviours

	private void Chase () {
		_motor.Pursuit(_game_manager.SoccerBall.gameObject);
	}

	private void Dribble () {
		_game_manager.SoccerBall.transform.position = transform.position + transform.forward;
		_motor.Seek(_team.OtherTeam.Goal.transform.position); //seek opposing teams goal

		// Avoid opponets
		Collider[] opponents = Physics.OverlapSphere(transform.position, fleeRadius, _team.OpponetLayerMask);

		foreach (var opponent in opponents) {
			_motor.Flee(opponent.transform.position, fleeSpeed);
		}
	}

	void Support () {

		DefaultSeek();

		/**
		 *  Have the player deside if hes in a good position to recieve the ball.
		*/
		Vector3 ball_to_self = transform.position - _game_manager.SoccerBall.transform.position;

		RaycastHit hit;
		Ray ray = new Ray(_game_manager.SoccerBall.transform.position, ball_to_self);

		// a list of any opponents that are hindering a safe pass
		List<Collider> opponentsColliders = new List<Collider>();

		// if a player is between the player and the ball
		if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponetLayerMask)) {

			// add to list of opponents
			opponentsColliders.Add(hit.collider);

			// Anounce that a pass is not possible
			_eventCanRecieve.Invoke(gameObject, false);

		} else { // if no player is directly betewwn the player and the ball

			bool hitRight = false;
			bool hitLeft = false;

			/**
			 *  Check for opponets to either side
			*/

			Vector3 ball_to_self_normal = new Vector3(ball_to_self.z, 0, -ball_to_self.x).normalized;
			ray.direction += ball_to_self_normal * sphereCastRadius;

			if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponetLayerMask)) {

				opponentsColliders.Add(hit.collider);

				hitRight = true;
			}

			ray.direction -= ball_to_self_normal * sphereCastRadius * 2;

			if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponetLayerMask)) {

				opponentsColliders.Add(hit.collider);

				hitLeft = true;
			}

			if (hitLeft && hitRight) { // if opponents on both sides
				_eventCanRecieve.Invoke(gameObject, false);
			} else if (hitLeft != hitRight) { // if opponets on one side
				_eventCanRecieve.Invoke(gameObject, true);
			} else { // if no opponets
				_eventCanRecieve.Invoke(gameObject, true);
			}

		}

		// For any player that is hindering a safe pass use a inverse hide behavior to get to a good position.
		foreach (Collider opponent in opponentsColliders) {
			Vector3 ball_to_opponent = opponent.transform.position - _game_manager.SoccerBall.transform.position;

			ball_to_opponent *= ball_to_self.magnitude / ball_to_opponent.magnitude;

			Vector3 steering = transform.position - (_game_manager.SoccerBall.transform.position + ball_to_opponent);

			steering *= (1 / steering.magnitude) * sphereCastRadius * 2;

			_motor.AddMovement(steering);

		}

	}

	void Pass () {

		Rigidbody rb = null;

		foreach (var p in _team.GetPlayersByAggression()) {
			if (p.gameObject == gameObject) continue;
			if (_can_pass_to.Contains(p.gameObject)) {
				p.IsBeingPassedBall = true;
				rb = p.GetComponent<Rigidbody>();
				break;
			}
		}

		if (rb == null) return;

		float T = Vector3.Distance(transform.position, _game_manager.SoccerBall.transform.position) / ballPassSpeed;
		Vector3 newTarget = _game_manager.SoccerBall.transform.position + rb.velocity * T;

		for (int i = 0; i < ballPassSteps; i++) {
			T = Vector3.Distance(transform.position, newTarget) / ballPassSpeed;
			newTarget = _game_manager.SoccerBall.transform.position + rb.velocity * T;
		}

		Vector3 passDirection = newTarget - transform.position;



		KickBall(passDirection);

	}

	void KickAction () {
		KickBall((GetBestShot() - _game_manager.SoccerBall.transform.position), _kickForce);
		HasBall = false;
	}

	void Idle () {
		Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
		_motor.Seek(defaultPos);
	}

	void StunTick () {
		if (stunDuration >= stunLimit) {
			Stunned = false;
			stunDuration = 0;
			return;
		}
		stunDuration += Time.deltaTime;
	}

	void Receive () {
		// Pursuit the ball
		_motor.Pursuit(_game_manager.SoccerBall.gameObject);
	}

	#endregion

	Vector3 GetBestShot () {
		GameObject goal = _team.OtherTeam.Goal;
		GameObject ball = _game_manager.SoccerBall.gameObject;
		Collider col = goal.GetComponent<Collider>();

		float goalWidth = col.bounds.size.z - col.bounds.size.z / 10; //Goal width with a small offset so the AI does not hit the edges of the goal.
		float initialOffset = goalWidth/2;

		Vector3 ballOffset = new Vector3(0, 0, ball.GetComponent<Collider>().bounds.size.z / 2);

		int precision = 10;
		float interval = goalWidth/(precision - 1);

		//First index is the target index. Amount of targets is defined by the precision.
		//Second index has two values. The first value represents how many of the three rayscasts on that target that actually hit something.
		//The second value represents how far away (index difference) this target is from a target that actually hit something with it's raycast.
		//This is all used for selecting the optimal target.
		int [,] targetInfo = new int [precision, 2];

		int lastCollision = -1;
		int bestTarget = (precision - 1) / 2;

		//Check for collision
		for (int i = 0; i < precision; i++) {
			Vector3 target = goal.transform.position - new Vector3(0, 0, initialOffset - interval * i);
			target = target - ball.transform.position;

			targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position, target, target.magnitude, _team.OpponetLayerMask));
			targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position - ballOffset, target, target.magnitude, _team.OpponetLayerMask));
			targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position + ballOffset, target, target.magnitude, _team.OpponetLayerMask));

			//Update index difference
			if (targetInfo[i, 0] > 0) {
				for (int j = i - 1; j > lastCollision; j--) {
					if (i - j < targetInfo[j, 1]) {
						targetInfo[j, 1] = i - j;
					}
				}
				targetInfo[i, 1] = 0;
				lastCollision = i;
			} else {
				targetInfo[i, 1] = i - lastCollision;
			}

			Debug.DrawRay(ball.transform.position, target, Color.red, 1, false);
			Debug.DrawRay(ball.transform.position - ballOffset, target, Color.red, 1, false);
			Debug.DrawRay(ball.transform.position + ballOffset, target, Color.red, 1, false);
		}

		for (int i = 1; i < precision; i++) {
			if (targetInfo[i, 0] < targetInfo[bestTarget, 0]) {
				bestTarget = i;
			} else if (targetInfo[i, 0] == targetInfo[bestTarget, 0] && targetInfo[i, 1] > targetInfo[bestTarget, 1]) {
				bestTarget = i;
			}
		}
		return goal.transform.position - new Vector3(0, 0, initialOffset - interval * bestTarget);
	}

	public override void KickOff () {
		_state = States.Idle;
		HasBall = false;
	}

	public override void ApplyStun () {
		_state = States.Stunned;
		Stunned = true;
		_motor.Stop();
	}
}