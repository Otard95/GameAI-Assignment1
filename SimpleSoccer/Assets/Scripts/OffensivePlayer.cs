using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OffensivePlayer : Player
{

    enum States
    {
        Idle,           //Default state. Goes back to start position and wait for kickoff.
        Chase,          //Chase after the ball and try to take it.
        Drible,         //Move with the ball.
        Recieve,        //Standing by to recieve the ball.
        Support,        //Move to a good position for recieving the ball.
        Kick,           //Shoot at goal.
        Pass            //Pass the ball.
    };

    Rigidbody _rigidBody;

    States _state;
    float _speed;
    float _kickForce;

    //TEMP
    bool _inPosition;
    //ENDTEMP

    new void Start()
    {
        base.Start();
        _rigidBody = GetComponent<Rigidbody>();
        _speed = 10;
        _state = States.Idle;
        _kickForce = 50;
    }

    // Update is called once per frame
    public void Update()
    {

        Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
        _motor.Seek(defaultPos);

        if (_has_ball)
        {
            if (_game_manager.Ball != null)
            {
                float distanceToBall = Vector3.Distance(transform.position, _game_manager.Ball.transform.position);

                if (distanceToBall > 10)
                {
                    _has_ball = false;
                }
            }
        }

        #region State Transitions
        //State transition
        switch (_state)
        {
            case States.Idle:
                {
                    if (_team.HasBall)
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
                    if (_team.HasBall)
                    {
                        _state = States.Support;
                    }
                    break;
                }
            case States.Drible:
                {
                    if (!HasBall)
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
                    if (!HasBall)
                    {
                        if (_team.HasBall)
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
                    if (HasBall)
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
                    if (!_team.HasBall)
                    {
                        _state = States.Chase;
                    }
                    else if (HasBall)
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
                    Drible();
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
        Vector3 newDirection = Vector3.RotateTowards(transform.position, _game_manager.Ball.transform.position, 0, 0.0f);

        if (Vector3.Angle(transform.forward, _game_manager.Ball.transform.position - transform.position) == 0)
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
        _motor.Pursuit(_game_manager.Ball);
    }

    private void Drible()
    {
        _motor.Seek(_team.OtherTeam.transform.position); //seek opposing teams goal
    }

    private void KickBall()
    {
        Rigidbody rb = _game_manager.Ball.GetComponent<Rigidbody>();
        //Vector3 direction = target.transform.position - transform.position;

        rb.AddForce(transform.forward * _kickForce, ForceMode.Force);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == _game_manager.Ball)
        {
            Rigidbody ballRigidbody = _game_manager.Ball.GetComponent<Rigidbody>();
            _state = States.Drible;
            HasBall = true;

            ballRigidbody.velocity = Vector3.zero;
            _rigidBody.velocity = Vector3.zero;
        }
    }
}