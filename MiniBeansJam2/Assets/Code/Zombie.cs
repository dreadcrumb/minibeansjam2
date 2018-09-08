using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
	public Vector3 AreaCenter;

	public GameObject Target;
	public double AttackSpeed = 1;
	public int ZombificationFactor = 5;
	public int Damage = 10;

	public ZombieState zombieState;
	public int searchTime;

	private SearchTimer searchTimer;
	private double _lastTargetUpdateTick = 0;
	private Vector3 _currentWaypoint;
	private NavMeshAgent _agent;
	private double _attackCooldown;

	// Use this for initialization
	void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		zombieState = ZombieState.IDLE;
		searchTimer = new SearchTimer();
	}

	// Update is called once per frame
	void Update()
	{
		if (zombieState == ZombieState.IDLE)
		{
			if (!_agent.hasPath)
			{
				WaitTime += Time.deltaTime;
				if (NextWaypointStart <= WaitTime)
				{
					MoveToNextPosition();
				}
			}
		}
		else if (zombieState == ZombieState.FOLLOWING)
		{
			_lastTargetUpdateTick += Time.deltaTime;
			if (_lastTargetUpdateTick > 1.0)
			{
				_agent.SetDestination(Target.transform.position);
				_lastTargetUpdateTick = 0;
			}
		}
		else if (zombieState == ZombieState.ALARMED)
		{
			_lastTargetUpdateTick += Time.deltaTime;
			// TODO: timer for when zombies stop looking
			if (!searchTimer.IsTimerRunning())
			{
				searchTimer.StartTimer();
			}
			else if (searchTimer.GetElapsed() > searchTime)
			{
				var x = searchTimer.GetElapsed();
				searchTimer.StopTimer();
				searchTimer.ResetTimer();
				zombieState = ZombieState.IDLE;
			}

			if (_lastTargetUpdateTick > 1.0)
			{
				LookAround();
			}

		}
		_attackCooldown = Math.Max(_attackCooldown - Time.deltaTime, -0.01);
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
			result = AreaCenter + Random.insideUnitSphere * AreaRange;
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
		player.ZombificationLevel += ZombificationFactor;
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
}