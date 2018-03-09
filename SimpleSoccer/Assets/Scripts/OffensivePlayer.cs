using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum States
{
	idle,
	chaseBall,
	drible,
	recieveBall,
	support,
	returnHome,
	kickBall
};

public class OffensivePlayer : Player {

	[SerializeField] float defaultOffenciveScalar = -5;
	[SerializeField] float defaultRightScalar = -10;

	HumanoidMotor _motor;

	States _state;

	//TEMP
	bool _teamGotBall;
	bool _gotBall;
	bool _inPosition;
	//ENDTEMP

	void Start () {
		_motor = GetComponent<HumanoidMotor>();
		_state = States.idle;
	}

	// Update is called once per frame
	public override void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);

		switch(_state)
		{
			case States.idle:
			{
				if(_teamGotBall)
				{
					_state = States.support;
				}
				else
				{
					_state = States.chaseBall;
				}
				break;
			}
			case States.chaseBall:
			{
				if(_teamGotBall)
				{
					_state = States.support;
				}
				break;
			} 
			case States.drible:
			{
				if(!_gotBall)
				{
					_state = States.chaseBall;
				}
				else if (_inPosition)
				{
					_state = States.kickBall;
				}
				break;
			} 
			case States.kickBall:
			{
				if(!_gotBall)
				{
					if(_teamGotBall)
					{
						_state = States.support;
					}
				}
				break;
			} 
			case States.recieveBall:
			{
				if(_gotBall)
				{
					_state = States.drible;
				}
				break;
			}
			case States.returnHome:
			{

				break;
			}
			case States.support:
			{
				if(!_teamGotBall)
				{
					_state = States.chaseBall;
				}
				else if (_gotBall)
				{
					_state = States.drible;
				}
				break;
			} 
		}
	}
}
