using UnityEngine;

public class AoEOnImpact : MonoBehaviour
{
	public float Radius = 3;
	public LayerMask TargetMask;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			DoAOE();
			Destroy(gameObject);
		}
	}

	private void DoAOE()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, Radius, TargetMask);
		foreach (var targetCollider in targetsInViewRadius)
		{
			var targetZombie = targetCollider.gameObject;
			Destroy(targetZombie);
		}
	}
}
