using JetBrains.Annotations;
using UnityEngine;

public class KeeperPlayer : Player {

	enum States {
		Idle,   // The player is idle and has no goal
		Block,  // The player will try to block the opponents shots
		Catch,  // Try to get the ball
		Pass,   // Will pass to a nearby friendly player
		Stunned // Just lost the ball. Need some time to figure things out.
	}

	[SerializeField] float catchRange = 3;

	States _current_state = States.Idle;

	[UsedImplicitly]
	new void Start () {
		base.Start();
	}

	// Update is called once per frame
	[UsedImplicitly]
	void Update () {

		// Special case where if keeper has the ball
		// The game goes to a kickoff state and players return to theis positions but the keeper still holds the ball
		// THen he is ready he will pass the ball to a team player.

		if (_has_ball && !_game_manager.IsKickoff) {
			_team.KickOff();
			_team.OtherTeam.KickOff();
		}

		switch (_current_state) {
			case States.Idle:
				// Player does nothing
				IdleTransitions();
				break;
			case States.Block:
				Block();
				BlockTransitions();
				break;
			case States.Catch:
				Catch();
				CatchTransitions();
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

	void IdleTransitions () {
		if (!_game_manager.IsKickoff) {
			_current_state = States.Block;
		}
	}

	void BlockTransitions () {
		if ((_game_manager.SoccerBall.transform.position - transform.position).magnitude < catchRange)
			_current_state = States.Catch;
	}

	void CatchTransitions () {
		if (!_team.HasBall) _current_state = States.Block;
		if ((_game_manager.SoccerBall.transform.position - _team.Goal.transform.position).magnitude > catchRange * 1.5)
			_current_state = States.Block;
		if (_has_ball) _current_state = States.Pass;
	}

	void PassTransitions () {
		if (!_has_ball) _current_state = States.Block;
	}

	void StunnedTransitions () {
		//if no longer disoriented go support
		if (!_stunned) {
			_current_state = States.Block;
			
		}
	}

	#region State Actions

	void Block () {

		DefaultSeek();

		Vector3 goal_to_ball = _game_manager.SoccerBall.transform.position - _team.Goal.transform.position;
		Vector3 goal_to_self = transform.position - _team.Goal.transform.position;

		goal_to_ball *= goal_to_self.magnitude / goal_to_ball.magnitude;

		Vector3 desired_direction = (_team.Goal.transform.position + goal_to_ball) - transform.position;
		_motor.AddMovement(desired_direction * 10);

	}

	void Catch () {
		_motor.Seek(_game_manager.SoccerBall.transform.position);
	}

	void Pass () {

		_game_manager.SoccerBall.transform.position = transform.position + transform.forward;

		if (Physics.OverlapSphere(transform.position, 20, _team.OpponentLayerMask).Length == 0) {
			// Find Best player to pass to.
			var players = _team.GetPlayersByAggression();
			Player pass_to = players[players.Length - 1];
			if (pass_to == this) pass_to = players[players.Length - 2];

			// Look at that player and kick the ball
			transform.LookAt(pass_to.transform);
			_game_manager.SoccerBall.transform.position = transform.position + transform.forward;
			KickBall(pass_to.transform.position - transform.position);
			
			// Disable kickoff state
			_game_manager.IsKickoff = false;

			// Notify teammate that its being passed the ball
			pass_to.IsBeingPassedBall = true;
			
			_has_ball = false;
			_stunned = true;

		}

	}

	void StunTick () {
		if (_stunDuration >= _stunLimit) {
			_stunned = false;
			_stunDuration = 0;
			return;
		}
		_stunDuration += Time.deltaTime;
	}

	#endregion

	public override void KickOff () {

		SeekDefaultPosition();
		_current_state = States.Idle;
		if (_has_ball) _current_state = States.Pass;

	}

	public override void ApplyStun () {}
}
