using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
	public const String ITEM_TAG = "Item";
	public const String GROUND_TAG = "Ground";

	public Vector3 CurrentTarget;
	public double InteractionRange = 1;
	public double ThrowRange = 4;
	[Range(0, 100)]
	public double ZombificationLevel = 0;
	[Range(0, 100)]
	public int Health = 100;
	public GameObject Trap;
	public GameObject Stone;
	public double ZombificationPassiveIncrement;

	private NavMeshAgent _agent;
    private PlayerActionIntention _intention;
	private bool _selected;

	public Dictionary<ItemType, int> Items = new Dictionary<ItemType, int>();

	// Use this for initialization
	void Start()
	{
		Items[ItemType.PILLS] = 0;
		Items[ItemType.TRAPS] = 0;
		Items[ItemType.STONES] = 3;
		_agent = GetComponent<NavMeshAgent>();
	}

    // Update is called once per frame
    void Update()
    {
	    if (!IsAlive())
	    {
		    return;
		    // TODO on animation finish delete?
	    }
	    
	    if (_intention != null)
	    {
		    if (!_intention.Update())
		    {
			    _intention = null;
		    }
	    }

	    if (_selected)
	    {
		    Ray clickRay;
		    RaycastHit hit;
		    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		    {
			    clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			    if (Physics.Raycast(clickRay, out hit))
			    {
				    var colliderGameObject = hit.collider.gameObject;
				    if (colliderGameObject.CompareTag(ITEM_TAG))
				    {
					    PickUpItemIfInRange(colliderGameObject);
				    }
				    else if (colliderGameObject.CompareTag(GROUND_TAG))
				    {
					    MoveTo(hit.point);
				    }
			    }
		    }
	    }

	    ZombificationLevel += Math.Min(ZombificationPassiveIncrement, 100);
    }

	public void MoveTo(Vector3 location)
	{
		CurrentTarget = location;
		_agent.SetDestination(CurrentTarget);
		_intention = null;
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		if (!IsAlive())
		{
			// TODO death animation
		}
	}

	public void PickUpItemIfInRange(GameObject colliderGameObject)
	{
		if (IsInRange(colliderGameObject.transform.position, InteractionRange))
		{
			PickUpItem(colliderGameObject);
			_intention = null;
		}
		else
		{
			_intention = new PlayerPickupActionIntention(this, colliderGameObject);
			_intention.Start();
		}
	}

	public void PickUpItem(GameObject colliderGameObject)
	{
		// TODO pickup animation
		var itemObject = colliderGameObject.GetComponent<ItemObject>();
		Items[itemObject.Type] += 1;
		Destroy(colliderGameObject);
	}

	private bool IsInRange(Vector3 location, double range)
	{
		return Vector3.Distance(transform.position, location) < range;
	}

    public void PlaceTrapAtIfInRange(Vector3 location)
    {
	    if (IsInRange(location, InteractionRange))
	    {
		    PlaceTrapAt(location);
	    }
	    else
	    {
		    _intention = new PlayerPlaceTrapActionIntention(this, location);
		    _intention.Start();
	    }
    }

    public void PlaceTrapAt(Vector3 location)
    {
	    if (Items[ItemType.TRAPS] <= 0)
	    {
		    return;
	    }
	    
	    // TODO place animation
	    Instantiate(Trap, location, transform.rotation);
	    Items[ItemType.TRAPS] -= 1;
    }

	public void SetSelected(bool selected)
	{
		_selected = selected;
	}

	public bool IsAlive()
	{
		return Health > 0;
	}

	public bool IsSelected()
	{
		return _selected;
	}

	public void TakePill()
	{
		Items[ItemType.PILLS] -= 1;
		ZombificationLevel = Math.Max(ZombificationLevel - 20, 0);
	}

	public void ThrowStoneIfInRange(Vector3 location)
	{
		if (IsInRange(location, ThrowRange))
		{
			ThrowStone(location);
		}
		else
		{
			_intention = new PlayerThrowStoneActionIntention(this, location, ThrowRange);
			_intention.Start();
		}
	}

	public void ThrowStone(Vector3 location)
	{
		var targetVector = location - transform.position;
		var stone = Instantiate(Stone, transform.position + new Vector3(0, 1, 0), transform.rotation);
		stone.GetComponent<Rigidbody>().AddForce(targetVector);
	}
}