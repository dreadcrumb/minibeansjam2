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
    public double PickupRange = 1;
    public int ZombificationLevel = 0;
    public int Health = 100;
    public GameObject SelectedItem;
    
    private NavMeshAgent _agent;
    private bool _hasFinished = true;

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(clickRay, out hit))
            {
                var colliderGameObject = hit.collider.gameObject;
                if (colliderGameObject.CompareTag(ITEM_TAG))
                {
                    PickUpItem(colliderGameObject);
                }
                else if (colliderGameObject.CompareTag(GROUND_TAG))
                {
                    CurrentTarget = hit.point;
                    _agent.SetDestination(CurrentTarget);
                    _hasFinished = false;
                }
            }
        }

        if (!_hasFinished && !_agent.hasPath) // TODO there should be a better way!
        {
            _hasFinished = true;
            OnPathFinish();
        }
    }

    private void OnPathFinish()
    {
        if (SelectedItem != null && IsInRange(SelectedItem))
        {
            AddItem(SelectedItem);
            SelectedItem = null;
        }
    }

    private void PickUpItem(GameObject colliderGameObject)
    {
        if (IsInRange(colliderGameObject))
        {
            AddItem(colliderGameObject);
        }
        else
        {
            _agent.SetDestination(colliderGameObject.transform.position);
            SelectedItem = colliderGameObject;
            _hasFinished = false;
        }
    }

    private void AddItem(GameObject colliderGameObject)
    {
        // TODO pickup animation
        var itemObject = colliderGameObject.GetComponent<ItemObject>();
        Items[itemObject.Type] += 1;
        Destroy(colliderGameObject);
    }

    private bool IsInRange(GameObject otherObject)
    {
        return Vector3.Distance(transform.position, otherObject.transform.position) < PickupRange;
    }
}