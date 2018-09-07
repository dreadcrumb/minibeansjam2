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
    
    private double _lastTargetUpdateTick = 0;
    private Vector3 _currentWaypoint;
    private NavMeshAgent _agent;
    private double _attackCooldown;

    // Use this for initialization
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
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
        else
        {
            _lastTargetUpdateTick += Time.deltaTime;
            if (_lastTargetUpdateTick > 1)
            {
                _agent.SetDestination(Target.transform.position);
                _lastTargetUpdateTick = 0;
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

    private void MoveToNextWayPoint()
    {
        CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.Count;
        _currentWaypoint = Waypoints[CurrentWaypointIndex];
        _agent.SetDestination(_currentWaypoint);
        WaitTime = 0;
        NextWaypointStart = Random.Range(MinWaitTime, MaxWaitTime);
    }

    private void MoveToNextPositionInArea()
    {
        var result = new Vector3(Random.Range(-AreaRange, AreaRange), 0, Random.Range(-AreaRange, AreaRange));
        result += AreaCenter;
        NavMeshHit hit;
        NavMesh.SamplePosition(result, out hit, AreaRange, 1);
        _agent.SetDestination(hit.position);
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
        player.Health -= Damage;
    }

    private bool CanAttack()
    {
        return _attackCooldown <= 0;
    }

    private bool CanAttack( GameObject other )
    {
        return CanAttack() && other.gameObject.CompareTag(PLAYER_TAG);
    }

    private void OnTriggerStay(Collider other)
    {
        if (CanAttack(other.gameObject))
        {
            Attack(other.gameObject);
        }
    }
}