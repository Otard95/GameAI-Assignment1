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
	}

	// Update is called once per frame
	public void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);

		_state = States.idle;

		//State transition
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

		//State action
		switch (_state)
		{
			case States.idle:
			{
				
				break;
			}
			case States.chaseBall:
			{
				
				break;
			} 
			case States.drible:
			{
				
				break;
			} 
			case States.kickBall:
			{
				
				break;
			} 
			case States.recieveBall:
			{
				
				break;
			}
			case States.returnHome:
			{

				break;
			}
			case States.support:
			{
				
				break;
			} 
		}
	}

	private void RecieveBall()
	{

	}

	private void KickBall()
	{

	}
}
