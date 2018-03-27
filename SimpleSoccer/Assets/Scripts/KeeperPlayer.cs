using JetBrains.Annotations;
using UnityEngine;

public class KeeperPlayer : Player {

	enum State {
		Idle,
		Block,
		Catch,
		Pass
	}

	[SerializeField] float catchRange = 3;

	State _current_state = State.Idle;

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

		if (_has_ball) {
			_team.KickOff();
			_team.OtherTeam.KickOff();
		}

		switch (_current_state) {
			case State.Idle:
				// Player does nothing
				IdleTransitions();
				break;
			case State.Block:
				Block();
				BlockTransitions();
				break;
			case State.Catch:
				Catch();
				CatchTransitions();
				break;
			case State.Pass:
				Pass();
				PassTransitions();
				break;
		}

	}

	void IdleTransitions () {
		if (!_game_manager.IsKickoff) {
			_current_state = State.Block;
		}
	}

	void BlockTransitions () {
		if ((_game_manager.SoccerBall.transform.position - transform.position).magnitude < catchRange)
			_current_state = State.Catch;
	}

	void CatchTransitions () {
		if (!_team.HasBall) _current_state = State.Block;
		if ((_game_manager.SoccerBall.transform.position - _team.Goal.transform.position).magnitude > catchRange * 1.5)
			_current_state = State.Block;
		if (_has_ball) _current_state = State.Pass;
	}

	void PassTransitions () {
		if (!_has_ball) _current_state = State.Block;
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
		_motor.Pursuit(_game_manager.SoccerBall.gameObject);
	}

	void Pass () {



	}

	#endregion

	public override void KickOff () {

		SeekDefaultPosition();
		_current_state = State.Idle;
		if (_has_ball) _current_state = State.Pass;

	}
}
