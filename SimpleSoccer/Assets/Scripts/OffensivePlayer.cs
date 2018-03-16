using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum States
{
	Idle,
	ChaseBall,
	Drible,
	Recieve,
	Support,
	KickBall
};

public class OffensivePlayer : Player {
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
					_state = States.ChaseBall;
				}
				break;
			}
			case States.ChaseBall:
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
					_state = States.ChaseBall;
				}
				else if (_inPosition)
				{
					_state = States.KickBall;
				}
				break;
			} 
			case States.KickBall:
			{
				if(!_gotBall)
				{
					if(_teamGotBall)
					{
						_state = States.Support;
					}
					else
					{
						_state = States.ChaseBall;
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
					_state = States.ChaseBall;
				}
				break;
			}			
			case States.Support:
			{
				if(!_teamGotBall)
				{
					_state = States.ChaseBall;
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
			case States.ChaseBall:
			{
				
				break;
			} 
			case States.Drible:
			{
				
				break;
			} 
			case States.KickBall:
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