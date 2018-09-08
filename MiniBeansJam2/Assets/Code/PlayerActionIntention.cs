using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerActionIntention
{
    protected Player _player;
    protected NavMeshAgent _agent;

    public PlayerActionIntention(Player player)
    {
        _player = player;
        _agent = player.gameObject.GetComponent<NavMeshAgent>();
    }

    public abstract void Start();
    
    public abstract bool Update();

    protected bool FinishedPath()
    {
        return !_agent.pathPending && _agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.remainingDistance < _player.InteractionRange;
    }
}

public class PlayerPickupActionIntention : PlayerActionIntention
{
    private GameObject _item;

    public PlayerPickupActionIntention(Player player, GameObject item) : base(player)
    {
        _item = item;
    }

    public override void Start()
    {
        _agent.SetDestination(_item.transform.position);
    }

    public override bool Update()
    {
        if (FinishedPath())
        {
            _player.PickUpItem(_item);
            return false;
        }

        return true;
    }
}

public class PlayerPlaceTrapActionIntention : PlayerActionIntention
{
    private Vector3 _location;
    public PlayerPlaceTrapActionIntention(Player player, Vector3 location) : base(player)
    {
        _location = location;
    }

    public override void Start()
    {
        _agent.SetDestination(_location);
    }

    public override bool Update()
    {
        if (FinishedPath())
        {
            _player.PlaceTrapAt(_location);
            return false;
        }

        return true;
    }
}