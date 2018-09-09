using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Zombie : MonoBehaviour
{
	public const String PLAYER_TAG = "Player";

	public WanderMode WanderMode = WanderMode.WAYPOINTS;
	public int MinWaitTime = 5;
	public int MaxWaitTime = 10;
	public double WaitTime = 0;

	public List<Vector3> Waypoints;
	public int CurrentWaypointIndex = 0;
	public double NextWaypointStart = 3.0;

	public float AreaRange = 5;

	public GameObject Target;
	public double AttackSpeed = 1;
	public int ZombificationFactor = 5;
	public int Damage = 10;
	public int searchTime;
	public int FollowBuffer;

	public Image QuestionMarkOutline;
	public Image QuestionMarkFill;
	public Image ExclamationMOutline;
	public Image ExclamationMFill;

	private Vector3 _areaCenter;
	public SearchTimer questionMarkTimer;
	public SearchTimer exclamationMarkTimer;
	private ZombieState zombieState;
	private double _lastTargetUpdateTick = 0;
	private Vector3 _currentWaypoint;
	private NavMeshAgent _agent;
	private double _attackCooldown;

	private AudioSource _source;

	// Use this for initialization
	void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		zombieState = ZombieState.IDLE;
		_areaCenter = transform.position;
		SetMarkVisibilityAndPosition(false, false);

		_source = GetComponent<AudioSource>();
		_source.pitch = Random.Range(0.8f, 1.2f);
		_source.Play();
	}

	// Update is called once per frame
	void Update()
	{
		switch (zombieState)
		{
			case (ZombieState.IDLE):
				if (!_agent.hasPath)
				{
					WaitTime += Time.deltaTime;
					if (NextWaypointStart <= WaitTime)
					{
						MoveToNextPosition();
					}
				}

				// hide question and exclamation mark
				SetMarkVisibilityAndPosition(false, false);

				break;
			case ZombieState.FOLLOWING:
				if (!exclamationMarkTimer.IsTimerRunning())
				{
					exclamationMarkTimer.StartTimer();
				}
				else if (exclamationMarkTimer.GetElapsed() > FollowBuffer)
				{
					var x = exclamationMarkTimer.GetElapsed();
					exclamationMarkTimer.StopTimer();
					exclamationMarkTimer.ResetTimer();
					zombieState = ZombieState.ALARMED;
					GetComponentInParent<FieldOfView>().ViewSpeed = 15;
				}
				//SetExclamationMarkFillLevel();

				_lastTargetUpdateTick += Time.deltaTime;
				if (_lastTargetUpdateTick > 1.0)
				{
					_agent.SetDestination(Target.transform.position);
					_lastTargetUpdateTick = 0;
				}

				// hide question mark and show exclamation mark
				SetMarkVisibilityAndPosition(false, true);

				break;
			case ZombieState.ALARMED:
				_lastTargetUpdateTick += Time.deltaTime;
				if (!questionMarkTimer.IsTimerRunning())
				{
					questionMarkTimer.StartTimer();
				}
				SetQestionMarkFillLevel();
				if (questionMarkTimer.GetElapsed() > searchTime)
				{
					var x = questionMarkTimer.GetElapsed();
					questionMarkTimer.StopTimer();
					questionMarkTimer.ResetTimer();
					zombieState = ZombieState.IDLE;
					GetComponentInParent<FieldOfView>().ViewSpeed = 9;
					QuestionMarkFill.fillAmount = 1;
				}



				if (_lastTargetUpdateTick > 1.0)
				{
					LookAround();
				}

				// hide exclamation mark and show question mark
				SetMarkVisibilityAndPosition(true, false);
				break;
		}

		_attackCooldown = Math.Max(_attackCooldown - Time.deltaTime, -0.01);
		bool isWalking = _agent.remainingDistance > _agent.radius;
		GetComponentInChildren<Footsteps>().SetFootstepsPlaying(isWalking);
		GetComponent<Animator>().SetBool("Walking", isWalking);
	}

	private void SetMarkVisibilityAndPosition(bool questionMarkVisibility, bool exclamationMarkVisibility)
	{
		QuestionMarkFill.enabled = questionMarkVisibility;
		QuestionMarkOutline.enabled = questionMarkVisibility;

		ExclamationMFill.enabled = exclamationMarkVisibility;
		ExclamationMOutline.enabled = exclamationMarkVisibility;

		Vector3 namePos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0, 1.5f));
		QuestionMarkFill.transform.position = namePos;
		QuestionMarkOutline.transform.position = namePos;
		ExclamationMFill.transform.position = namePos;
		ExclamationMOutline.transform.position = namePos;
	}

	public void SetQestionMarkFillLevel()
	{
		var elapsed = questionMarkTimer.GetElapsed();
		if (elapsed > 0)
		{
			QuestionMarkFill.fillAmount = 1 - elapsed / searchTime;
		}
		else
		{
			QuestionMarkFill.fillAmount = 1;
		}
	}

	private void MoveToNextPosition()
	{
		switch (WanderMode)
		{
			case WanderMode.AREA:
				MoveToNextPositionInArea();
				break;
			case WanderMode.WAYPOINTS:
				MoveToNextWayPoint();
				break;
		}
	}

	private void LookAround()
	{
		// TODO: make this good
		//int rnd = Random.Range(0, 100);
		//int rotation = Random.Range(0, 100);
		//if (rnd > 50)
		//{
		//	_agent.SetDestination(Vector3.left);
		//}
		//else
		//{
		//	_agent.SetDestination(Vector3.right);
		//}

	}

	private void MoveToNextWayPoint()
	{
		if (Waypoints.Count == 0)
		{
			return;
		}

		CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.Count;
		_currentWaypoint = Waypoints[CurrentWaypointIndex];
		_agent.SetDestination(_currentWaypoint);
		ResetWaitTime();
	}

	private void MoveToNextPositionInArea()
	{
		Vector3 result;
		NavMeshHit hit;
		do
		{
			result = _areaCenter + Random.insideUnitSphere * AreaRange;
		} while (!NavMesh.SamplePosition(result, out hit, 1, NavMesh.AllAreas));
		_agent.SetDestination(hit.position);
		ResetWaitTime();
	}

	private void ResetWaitTime()
	{
		WaitTime = 0;
		NextWaypointStart = Random.Range(MinWaitTime, MaxWaitTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CanAttack(other.gameObject))
		{
			Attack(other.gameObject);
		}
	}

	private void Attack(GameObject otherGameObject)
	{
		// TODO play attack animation
		_attackCooldown = AttackSpeed;
		var player = otherGameObject.GetComponent<Player>();
		player.TakeInfection(ZombificationFactor);
		player.TakeDamage(Damage);
	}

	private bool CanAttack()
	{
		return _attackCooldown <= 0;
	}

	private bool CanAttack(GameObject other)
	{
		return CanAttack() && other.CompareTag(PLAYER_TAG) && other.GetComponent<Player>().IsAlive();
	}

	private void OnTriggerStay(Collider other)
	{
		if (CanAttack(other.gameObject))
		{
			Attack(other.gameObject);
		}
	}

	public void SetAgentDestination(Vector3 dest)
	{
		_agent.SetDestination(dest);
	}

	public void SetZombieState(ZombieState zS)
	{
		if (zS == ZombieState.FOLLOWING)
		{
			questionMarkTimer.ResetTimer();
		}
		zombieState = zS;
	}

	public ZombieState GetZombieState()
	{
		return zombieState;
	}
}