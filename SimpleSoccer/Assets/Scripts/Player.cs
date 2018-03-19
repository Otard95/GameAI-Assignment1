using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> {
}

[RequireComponent(typeof(HumanoidMotor))]
public class Player : MonoBehaviour {

	protected bool _has_ball;
	public bool HasBall { protected set { _has_ball = value; } get { return _has_ball; } }

	[SerializeField] protected float defaultOffenciveScalar = -15;
	[SerializeField] protected float defaultRightScalar = -10;

	/**
	 * ## Public Fields
	*/

	public PlayerEvent CanRecieve;

	/**
	 * ## Protected Filds
	*/

	protected Transform _team_base_transform;
	protected Team _team;
	protected HumanoidMotor _motor;

	[UsedImplicitly]
	void Awake () {
		if (CanRecieve == null) CanRecieve = new PlayerEvent();
	}

	public void SetBasePos (Transform t) {
		_team_base_transform = t;
	}

	protected void Start () {
		_team = transform.parent.GetComponent<Team>();
		_motor = GetComponent<HumanoidMotor>();
	}

	public virtual void EventCanRecieve (Player player) {}

}
