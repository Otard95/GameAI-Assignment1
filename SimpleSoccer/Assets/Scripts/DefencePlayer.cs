using System.Collections;
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
		Pass     // The player has the ball and is trying to pass it to another player(offensive)
	}

	States _current_state;

	new void Start () {
		base.Start();
		GetComponent<Transform>();

		_current_state = States.Idle;
	}
	
	void Update () {

		//Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		//_motor.MoveToPoint(defaultPos);

		switch (_current_state) {
			case States.Idle:
				// Do nothing
				IdleTransition();
				break;

			case States.Dribble:
				Dribble();
				DribbleTransition();
				break;

			case States.Support:
				Support();
				SupportTransition();
				break;

			case States.Block:
				Block();
				BlockTransition();
				break;

			case States.Receive:
				Receive();
				ReceiveTransition();
				break;

			case States.Pass:
				Pass();
				PassTransition();
				break;
		}

	}

	/**
	 * ## Transitions
	*/

	void IdleTransition () {
		
	}

	void DribbleTransition () {

	}

	void SupportTransition () {

	}

	void BlockTransition () {

	}

	void ReceiveTransition () {

	}

	void PassTransition () {

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

