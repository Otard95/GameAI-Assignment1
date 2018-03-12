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
	Rigidbody _rigidBody;

	States _state;
	GameObject _ball;
	float _speed;
	float _kickForce;

	//TEMP
	bool _teamGotBall;
	bool _gotBall;
	bool _inPosition;
	//ENDTEMP

	void Start () {
		_rigidBody = GetComponent<Rigidbody>();
		_ball = null;
		_speed = 10;
		_state = States.idle;
		_kickForce = 50;
	}

	// Update is called once per frame
	public void Update () {

		Vector3 defaultPos = teamBaseTransform.position + (teamBaseTransform.forward * defaultOffenciveScalar) + (teamBaseTransform.right * defaultRightScalar);
		_motor.MoveToPoint(defaultPos);

		if(_gotBall)
		{
			float distanceToBall = Vector3.Distance(transform.position, _ball.transform.position);
			if(distanceToBall > 10)
			{
				_gotBall = false;
				_ball = null;
			}
		}

		#region State Transitions
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
					else
					{
						_state = States.chaseBall;
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
				else
				{
					_state = States.chaseBall;
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
		#endregion

		#region State Actions
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
				KickBall();
				break;
			} 
			case States.recieveBall:
			{
				RecieveBall();
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
	#endregion

	private void RecieveBall()
	{
		Vector3 newDirection = Vector3.RotateTowards(transform.position, _ball.transform.position, 0, 0.0f);

		if(Vector3.Angle(transform.forward, _ball.transform.position - transform.position) == 0)
		{
			_state = States.drible;
		}
		else
		{
			transform.rotation = Quaternion.LookRotation(newDirection);
		}		
	}

	private void KickBall()
	{
		Rigidbody rb = _ball.GetComponent<Rigidbody>();
		//Vector3 direction = target.transform.position - transform.position;

		rb.AddForce(transform.forward * _kickForce, ForceMode.Force);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Soccer Ball")
		{
			Rigidbody ballRigidbody = _ball.GetComponent<Rigidbody>();

			_state = States.recieveBall;
			_ball = collision.gameObject;

			_teamGotBall = true;
			_gotBall = true;

			ballRigidbody.velocity = Vector3.zero;
			_rigidBody.velocity = Vector3.zero;
		}
	}
}
