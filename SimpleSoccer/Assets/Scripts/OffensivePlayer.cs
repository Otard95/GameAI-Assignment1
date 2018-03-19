using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OffensivePlayer : Player {

	enum States
	{
		Idle, 			//Default state. Goes back to start position and wait for kickoff.
		Chase,			//Chase after the ball and try to take it.
		Drible,			//Move with the ball.
		Recieve,		//Standing by to recieve the ball.
		Support,		//Move to a good position for recieving the ball.
		Kick,			//Shoot at goal.
		Pass			//Pass the ball.
	};

	Rigidbody _rigidBody;

	States _state;
	float _speed;
	float _kickForce;

	//TEMP
	[SerializeField] GameObject _ball;
	[SerializeField] GameObject _goal;
	bool _teamGotBall;
	bool _gotBall;
	bool _inPosition;
	//ENDTEMP

	void Start () {
		_rigidBody = GetComponent<Rigidbody>();
		_ball = null;
		_speed = 10;
		_state = States.Idle;
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
			}
		}

		#region State Transitions
		//State transition
		switch(_state)
		{
			case States.Idle:
			{
				if(_teamGotBall)
				{
					_state = States.Support;
				}
				else
				{
					_state = States.Chase;
				}
				break;
			}
			case States.Chase:
			{
				if(_teamGotBall)
				{
					_state = States.Support;
				}
				break;
			} 
			case States.Drible:
			{
				if(!_gotBall)
				{
					_state = States.Chase;
				}
				else if (_inPosition)
				{
					_state = States.Kick;
				}
				break;
			} 
			case States.Kick:
			{
				if(!_gotBall)
				{
					if(_teamGotBall)
					{
						_state = States.Support;
					}
					else
					{
						_state = States.Chase;
					}
				}
				break;
			} 
			case States.Recieve:
			{
				if(_gotBall)
				{
					_state = States.Drible;
				}
				else
				{
					_state = States.Chase;
				}
				break;
			}			
			case States.Support:
			{
				if(!_teamGotBall)
				{
					_state = States.Chase;
				}
				else if (_gotBall)
				{
					_state = States.Drible;
				}
				break;
			} 
		}
		#endregion

		#region State Actions
		//State action
		switch (_state)
		{
			case States.Idle:
			{
				
				break;
			}
			case States.Chase:
			{
				ChaseBall();
				break;
			} 
			case States.Drible:
			{
				
				break;
			} 
			case States.Kick:
			{
				KickBall();
				break;
			} 
			case States.Recieve:
			{
				RecieveBall();
				break;
			}
			case States.Support:
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
			_state = States.Drible;
		}
		else
		{
			transform.rotation = Quaternion.LookRotation(newDirection);
		}		
	}

	private void ChaseBall()
	{
		_motor.Pursuit(_ball);
	}

	private void Drible()
	{
		
	}

	private void KickBall()
	{
		Rigidbody rb = _ball.GetComponent<Rigidbody>();
		//Vector3 direction = target.transform.position - transform.position;

		rb.AddForce(transform.forward * _kickForce, ForceMode.Force);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == _ball)
		{
			Rigidbody ballRigidbody = _ball.GetComponent<Rigidbody>();

			_state = States.Recieve;

			_teamGotBall = true;
			_gotBall = true;

			ballRigidbody.velocity = Vector3.zero;
			_rigidBody.velocity = Vector3.zero;
		}
	}
}