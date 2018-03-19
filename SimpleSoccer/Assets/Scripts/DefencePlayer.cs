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
		if (!_game_manager.IsKickoff) {
			_current_state = States.Support;
		}
	}

	void DribbleTransitions ()
	{
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

		// if player got the ball go to Dirbble state
		if (_has_ball) _current_state = States.Dribble;
	}

	void ReceiveTransitions () {
		// if opposing team got the ball during the pass go to Block state
		if (!_team.HasBall) _current_state = States.Block;
	}

	void PassTransitions () {
		// if opposing team got the ball during the pass go to Block state
		if (!_team.HasBall) _current_state = States.Support;
	}

	/**
	 * ## State Behaviors
	*/

	void Dribble () {

	}

	void Support () {

	}

	void Block () {

	}

	void Receive () {

	}

	void Pass () {

	}

}

