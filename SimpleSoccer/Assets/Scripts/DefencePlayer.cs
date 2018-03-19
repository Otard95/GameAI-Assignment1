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

	GameManager _game_manager;
	States _current_state;

	[UsedImplicitly]
	new void Start () {
		base.Start();
		GetComponent<Transform>();

		_game_manager = GameManager.Instance;
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

	void DribbleTransitions () {

	}

	void SupportTransitions () {
		if (!_team.HasBall) {
			_current_state = States.Block;
		}

	}

	void BlockTransitions () {

	}

	void ReceiveTransitions () {

	}

	void PassTransitions () {

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

