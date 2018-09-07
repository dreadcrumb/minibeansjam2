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
    public Camera Camera;
    public double PickupRange = 1;
    private NavMeshAgent _agent;

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
            Ray clickRay = Camera.ScreenPointToRay(Input.mousePosition);
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
                }
            }
        }
    }

    private void PickUpItem(GameObject colliderGameObject)
    {
        if (IsInRange(colliderGameObject.transform.position))
        {
            // TODO pickup animation
            var itemObject = colliderGameObject.GetComponent<ItemObject>();
            Items[itemObject.Type] += 1;
            Destroy(colliderGameObject);
        }
        else
        {
            _agent.SetDestination(colliderGameObject.transform.position);
            // TODO on finish pickup
        }
    }

    private bool IsInRange(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < PickupRange;
    }
}