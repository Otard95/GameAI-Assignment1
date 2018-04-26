using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DefencePlayer : Player {

	enum States {
		Idle,    // The player is idle and has no goal
		Dribble, // Player had the ball and is trying to get to an advantages position for a pass.
		Support, // Player's team has the ball. The player advances up the pitch with the team, but stays further back to defend.
		Block,   // The player will try to block the opponents passes
		Receive, // The player is being passed the ball, and activly tries to catch it.
		Pass,    // The player has the ball and is trying to pass it to another player(offensive)
		Stunned  // Just lost the ball. Need some time to figure things out.
	}

	/**
	 * ## Unity Proporties
	*/

	[SerializeField] float sphereCastRadius = 1.5f;

	/**
	 * ## Private Fields
	*/

	States _current_state;

	[UsedImplicitly]
	new void Start () {
		base.Start();
		GetComponent<Transform>();

		_current_state = States.Idle;
	}

	[UsedImplicitly]
	void Update () {

		//Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
		//_motor.MoveToPoint(defaultPos);

		switch (_current_state) {
			case States.Idle:
				// Do nothing
				IdleTransitions();
				break;

			case States.Dribble:
				Dribble();
				DribbleTransitions();
				break;

			case States.Support:
				Support();
				SupportTransitions();
				break;

			case States.Block:
				Block();
				BlockTransitions();
				break;

			case States.Receive:
				Receive();
				ReceiveTransitions();
				break;

			case States.Pass:
				Pass();
				PassTransitions();
				break;
			case States.Stunned:
				StunTick();
				StunnedTransitions();
				break;
		}

	}

	/**
	 * ## Transitions
	*/

	#region Transitions

	void IdleTransitions () {
		if (!_game_manager.IsKickoff) _current_state = States.Support;
	}

	void DribbleTransitions () {
		// if player lost the ball to an opponent go to Block state
		if (!_has_ball) _current_state = States.Block;
		// If an opponent gets to close try doing a pass
		if (Physics.OverlapSphere(transform.position, minOpponentDistForPass, _team.OpponentLayerMask).Length != 0 ||
				_rb.velocity.magnitude < 1) {
			_current_state = States.Pass;
		}
	}

	void SupportTransitions () {
		// if team lost the ball go to Block state
		if (!_team.HasBall) _current_state = States.Block;

		// if player got the ball go to Dribble state
		if (_has_ball) _current_state = States.Dribble;

		// if player is being passed the ball go to Recieve state
		if (IsBeingPassedBall) _current_state = States.Receive;
	}

	void BlockTransitions () {
		// if players team gets the ball support them
		if (_team.HasBall) _current_state = States.Support;

		// if player is being passed the ball go to Recieve state
		if (IsBeingPassedBall) _current_state = States.Receive;

		// if player got the ball go to Dirbble state
		if (_has_ball) _current_state = States.Dribble;
	}

	void ReceiveTransitions () {
		// if opposing team got the ball during the pass go to Block state
		if (_team.OtherTeam.HasBall) _current_state = States.Block;
		// if the player catches the ball go to Dribble
		if (_has_ball) _current_state = States.Dribble;
		// if player is no longer in Recieve state reset IsBeingPassedBall
		if (_current_state != States.Receive) IsBeingPassedBall = false;
	}

	void PassTransitions () {
		// player goes to Support state when ball is passed
		if (!_has_ball) _current_state = States.Support;
	}

	void StunnedTransitions () {
		//if no longer disoriented go support
		if (!_stunned) {
			if (_team.HasBall)
			{
				_current_state = States.Support;
			}
			else
			{
				_current_state = States.Block;
			}
			
		}
	}

	#endregion

	/**
	 * ## State Behaviors
	*/

	void Dribble () {

		_game_manager.SoccerBall.transform.position = transform.position + transform.forward;

		SeekDefaultPosition();

		_motor.AddMovement(_team.OtherTeam.transform.position - transform.position);

		// Avoid opponets
		Collider[] opponents = Physics.OverlapSphere(transform.position, fleeRadius, _team.OpponentLayerMask);

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
		if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponentLayerMask)) {

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

			if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponentLayerMask)) {

				opponentsColliders.Add(hit.collider);

				hitRight = true;
			}

			ray.direction -= ball_to_self_normal * sphereCastRadius * 2;

			if (Physics.SphereCast(ray, sphereCastRadius, out hit, ball_to_self.magnitude, _team.OpponentLayerMask)) {

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

	void Block () {
		// Use interpose to predict there to stand to block a pass

		GameObject targetB;

		if (defaultRightScalar < 0) {

			Player[] opponetPlayers = _team.OtherTeam.GetPlayersByAggression();
			if (opponetPlayers == null) {

				targetB = _team.Goal;

			} else {

				if (_game_manager.SoccerBall.Owner == null)
					targetB = opponetPlayers[0].gameObject;
				else
					targetB = opponetPlayers[0].gameObject != _game_manager.SoccerBall.Owner.gameObject ? opponetPlayers[0].gameObject : opponetPlayers[1].gameObject;

			}

		} else {
			targetB = _team.Goal;
		}

		if (_game_manager.SoccerBall.Owner != null) {
			_motor.Interpose(_game_manager.SoccerBall.Owner.gameObject, targetB);
		}

	}

	void Receive () {

		// Pursuit the ball
		_motor.Pursuit(_game_manager.SoccerBall.gameObject);

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

		transform.LookAt(rb.transform);
		_game_manager.SoccerBall.transform.position = transform.position + transform.forward;
		KickBall(passDirection);

		_has_ball = false;
		_stunned = true;

	}

	
	void StunTick () {
		if (_stunDuration >= _stunLimit) {
			_stunned = false;
			_stunDuration = 0;
			return;
		}
		_stunDuration += Time.deltaTime;
	}

	public override void KickOff () {
		SeekDefaultPosition();
		_current_state = States.Idle;
		_has_ball = false;
	}

	public override void ApplyStun () {
		_current_state = States.Stunned;
		_stunned = true;
		_motor.Stop();
	}
}

