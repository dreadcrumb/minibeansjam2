using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	public Vector3 CurrentTarget;
	public Camera Camera;
	private NavMeshAgent _agent;
	
	// Use this for initialization
	void Start ()
	{
		_agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!_agent.hasPath && Input.GetMouseButtonDown(0))
		{
			Ray clickRay = Camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(clickRay, out hit))
			{
				CurrentTarget = hit.point;
				_agent.SetDestination(CurrentTarget);
			}
		}
	}
}
