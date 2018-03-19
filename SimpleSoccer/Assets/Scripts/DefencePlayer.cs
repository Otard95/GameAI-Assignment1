using JetBrains.Annotations;
using UnityEngine;

public class DefencePlayer : Player {

	enum States {
		Idle,    // The player is idle and has no goal
		Dribble, // Player had the ball and is trying to get to an advantages position for a pass.
		Support, // Player's team has the ball. The player advances up the pitch with the team, but stays further back to defend.
		Block,   // The player will try to block the opponents passes
		Receive, // The player is being passed the ball, and activly tries to catch it.
		Pass     // The player has the ball and is trying to pass it to another player(offensive)
	}

	/**
	 * ## Unity Proporties
	*/

	[SerializeField] LayerMask sphereCastLayerMask;
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
		}

	}

	/**
	 * ## Transitions
	*/

	void IdleTransitions () {
		if (!_game_manager.IsKickoff) _current_state = States.Support;
	}

	void DribbleTransitions () {
		// if player lost the ball to an opponent go to Block state
		if (!_has_ball) _current_state = States.Block;
	}

	void SupportTransitions () {
		// if team lost the ball go to Block state
		if (!_team.HasBall) _current_state = States.Block;

		// if player got the ball go to Dirbble state
		if (_has_ball) _current_state = States.Dribble;

		// if player is being passed the ball go to Recieve state
		if (_is_being_passed_ball) _current_state = States.Receive;
	}

	void BlockTransitions () {
		// if player is being passed the ball go to Recieve state
		if (_is_being_passed_ball) _current_state = States.Receive;

		// if players team gets the ball support them
		if (_team.HasBall) _current_state = States.Support;

		// if player got the ball go to Dirbble state
		if (_has_ball) _current_state = States.Dribble;
	}

	void ReceiveTransitions () {
		// if opposing team got the ball during the pass go to Block state
		if (!_team.HasBall) _current_state = States.Block;
	}

	void PassTransitions () {
		// player goes to Support state when ball is passed
		if (!_has_ball) _current_state = States.Support;
	}

	/**
	 * ## State Behaviors
	*/

	void Dribble () {

	}

	void Support () {

		// Have the player deside if hes in a good position to recieve the ball.

		Vector3 ball_to_self = transform.position - _game_manager.ball.transform.position;

		Ray ray = new Ray(_game_manager.ball.transform.position, ball_to_self);
		if (Physics.SphereCast(ray, sphereCastRadius, ball_to_self.magnitude, sphereCastLayerMask)) {

			EventCanRecieve.Invoke(gameObject, false);

		} else {

			bool hitRight = false;
			bool hitLeft = false;

			Vector3 ball_to_self_normal = new Vector3(ball_to_self.z, 0, -ball_to_self.x).normalized;
			ray.direction += ball_to_self_normal * sphereCastRadius;

			if (Physics.SphereCast(ray, sphereCastRadius, ball_to_self.magnitude, sphereCastLayerMask)) {
				hitRight = true;
			}

			ray.direction -= ball_to_self_normal * sphereCastRadius * 2;

			if (Physics.SphereCast(ray, sphereCastRadius, ball_to_self.magnitude, sphereCastLayerMask)) {
				hitLeft = true;
			}

			if (hitLeft && hitRight) {
				EventCanRecieve.Invoke(gameObject, false);
			} else if (hitLeft != hitRight) {
				EventCanRecieve.Invoke(gameObject, true);
			} else {
				EventCanRecieve.Invoke(gameObject, true);
			}
		
		}

		// TODO: Implement hide behaviour but use the inverse of the result



	}

	void Block () {

	}

	void Receive () {

	}

	void Pass () {

	}

}

