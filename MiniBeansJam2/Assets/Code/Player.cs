using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	public const String ITEM_TAG = "Item";
	public const String GROUND_TAG = "Ground";

	public Vector3 CurrentTarget;
	public double InteractionRange = 1;
	public int ZombificationLevel = 0;
	public int Health = 100;
	public GameObject SelectedItem;
	public GameObject Trap;

	private NavMeshAgent _agent;
    private PlayerActionIntention _intention;

	public Dictionary<ItemType, int> Items = new Dictionary<ItemType, int>();

	// Use this for initialization
	void Start()
	{
		Items[ItemType.PILLS] = 0;
		Items[ItemType.TRAPS] = 0;
		_agent = GetComponent<NavMeshAgent>();
	}

    // Update is called once per frame
    void Update()
    {
	    if (_intention != null)
	    {
		    if (!_intention.Update())
		    {
			    _intention = null;
		    }
	    }
	    
        Ray clickRay;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
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
                    CurrentTarget = hit.point;
                    _agent.SetDestination(CurrentTarget);
	                _intention = null;
                }
            }
        }
        else if (Input.GetMouseButton(1) && Items[ItemType.TRAPS] > 0)
        {
            clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out hit))
            {
                var colliderGameObject = hit.collider.gameObject;
                if (colliderGameObject.CompareTag(GROUND_TAG))
                {
                    PlaceTrapAtIfInRange(hit.point);
                }
            }
        }
	}

	private void PickUpItemIfInRange(GameObject colliderGameObject)
	{
		if (IsInRange(colliderGameObject.transform.position))
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

	private bool IsInRange(Vector3 location)
	{
		return Vector3.Distance(transform.position, location) < InteractionRange;
	}

    private void PlaceTrapAtIfInRange(Vector3 location)
    {
	    if (IsInRange(location))
	    {
		    PlaceTrapAt(location);
	    }
	    else
	    {
		    _agent.SetDestination(location);
		    _intention = new PlayerPlaceTrapActionIntention(this, location);
		    _intention.Start();
	    }
    }

    public void PlaceTrapAt(Vector3 location)
    {
	    // TODO place animation
	    Instantiate(Trap, location, transform.rotation);
    }
}