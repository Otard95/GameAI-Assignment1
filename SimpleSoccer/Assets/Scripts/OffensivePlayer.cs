using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OffensivePlayer : Player
{
    enum States {
		Idle,       // The player is idle and has no goal
        Chase,      // The team does not have the ball, go get it.
		Dribble,    // Player had the ball and is trying to get to an advantages position for a pass.
		Support,    // Player's team has the ball. The player advances up the pitch with the team, but stays further back to defend.
		Receive,    // The player is being passed the ball, and activly tries to catch it.
		Pass,       // The player has the ball and is trying to pass it to another player(offensive)
        Kick,       // The player is in a position to score a goal, so he goes for it.
        Stunned     // Just lost the ball. Need some time to figure things out.
	}

    /**
	 * ## Private Fields
	*/
    float _speed;
    float _kickForce;
    States _state;

    new void Start()
    {
        base.Start();
        _speed = 10;
        _kickForce = 15;
        _state = States.Idle;
    }

    // Update is called once per frame
    public void Update()
    {
        if (_has_ball)
        {
            if (_game_manager.SoccerBall != null)
            {
                float distanceToBall = Vector3.Distance(transform.position, _game_manager.SoccerBall.transform.position);

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
                    if(!_game_manager.IsKickoff)
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
            case States.Stunned:
            {
                if(!Stunned)
                {
                    _state = States.Chase;
                }
                break;
            }
            case States.Chase:
                {
                    if(HasBall)
                    {
                        _state = States.Dribble;
                    }
                    else if (_team.HasBall)
                    {
                        _state = States.Support;
                    }
                    break;
                }
            case States.Dribble:
                {
                    if (!HasBall)
                    {
                        _state = States.Chase;
                    }
                    else if ((_team.OtherTeam.Goal.transform.position - transform.position).magnitude < 10)
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
            case States.Receive:
                {
                    if (HasBall)
                    {
                        _state = States.Dribble;
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
                        _state = States.Dribble;
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
                    Idle();
                    break;
                }
            case States.Stunned:
                {
                    StunTick();
                    break;
                }
            case States.Chase:
                {
                    ChaseBall();
                    break;
                }
            case States.Dribble:
                {
                    Drible();
                    break;
                }
            case States.Kick:
                {
                    KickAction();
                    break;
                }
            case States.Receive:
                {
                    break;
                }
            case States.Support:
                {

                    break;
                }
        }
    }
    #endregion    

    private void ChaseBall()
    {
        _motor.Pursuit(_game_manager.SoccerBall.gameObject);
    }

    private void Drible()
    {
        _game_manager.SoccerBall.transform.position = transform.position + transform.forward;
        _motor.Seek(_team.OtherTeam.Goal.transform.position); //seek opposing teams goal
    }

    private Vector3 GetBestShot()
    {
        GameObject goal = _team.OtherTeam.Goal;
        GameObject ball = _game_manager.SoccerBall.gameObject;
        Collider collider = goal.GetComponent<Collider>();

        float goalWidth = collider.bounds.size.z - collider.bounds.size.z / 10; //Goal width with a small offset so the AI does not hit the edges of the goal.
        float initialOffset = goalWidth/2;

        Vector3 ballOffset = new Vector3(0, 0, ball.GetComponent<Collider>().bounds.size.z / 2);

        int precision = 10;
        float interval = goalWidth/(precision - 1);

        //First index is the target index. Amount of targets is defined by the precision.
        //Second index has two values. The first value represents how many of the three rayscasts on that target that actually hit something.
        //The second value represents how far away (index difference) this target is from a target that actually hit something with it's raycast.
        //This is all used for selecting the optimal target.
        int [,] targetInfo = new int [precision, 2];

        int lastCollision = -1;
        int bestTarget = (precision - 1) / 2;

        //Check for collision
        for (int i = 0; i < precision; i++)
        {
            Vector3 target = goal.transform.position - new Vector3(0, 0, initialOffset - interval * i);
            target = target - ball.transform.position;

            targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position, target, target.magnitude, _team.OpponetLayerMask));
            targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position - ballOffset, target, target.magnitude, _team.OpponetLayerMask));
            targetInfo[i, 0] += Convert.ToInt32(Physics.Raycast(ball.transform.position + ballOffset, target, target.magnitude, _team.OpponetLayerMask));

            //Update index difference
            if(targetInfo[i, 0] > 0)
            {                
                for (int j = i - 1; j > lastCollision; j--)
                {
                    if(i - j < targetInfo[j, 1])
                    {
                        targetInfo[j, 1] = i - j;
                    }
                }
                targetInfo[i, 1] = 0;
                lastCollision = i;                
            }
            else if (lastCollision >= 0)
            {
                targetInfo[i, 1] = i - lastCollision;
            }

            Debug.DrawRay(ball.transform.position, target, Color.red, 1, false);
            Debug.DrawRay(ball.transform.position - ballOffset, target, Color.red, 1, false);
            Debug.DrawRay(ball.transform.position + ballOffset, target, Color.red, 1, false);
        }

        for (int i = 0; i < precision; i++)
        {
            if(targetInfo[i, 0] < targetInfo[bestTarget, 0])
            {
                bestTarget = i;
            }
            else if(targetInfo[i, 0] == targetInfo[bestTarget, 0] && targetInfo[i, 1] > targetInfo[bestTarget, 1])
            {
                bestTarget = i;
            }
        }
        return goal.transform.position - new Vector3(0, 0, initialOffset - interval * bestTarget);
    }

    void KickAction()
    {
        KickBall((GetBestShot() - _game_manager.SoccerBall.transform.position).normalized * _kickForce);
        HasBall = false;
    }

    void Idle()
    {
        Vector3 defaultPos = _team_base_transform.position + (_team_base_transform.forward * defaultOffenciveScalar) + (_team_base_transform.right * defaultRightScalar);
        _motor.Seek(defaultPos);
    }

    public override void  KickOff()
	{
		_state = States.Idle;
        HasBall = false;
	}

    public override void ApplyStun()
    {
        _state = States.Stunned;
        Stunned = true;
    }

    void StunTick()
    {
        if(stunDuration >= stunLimit)
        {
            Stunned = false;
            stunDuration = 0;
            return;
        }
        stunDuration += Time.deltaTime;
        return;
    }
}