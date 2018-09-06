using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	public Transform goal;
	
	// Use this for initialization
	void Start()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.destination = goal.position;
	}
}
