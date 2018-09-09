using UnityEngine;

public class Stone : MonoBehaviour
{
    public float Radius;
    public LayerMask TargetMask;
	private AudioSource _source;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, Radius, TargetMask);
            foreach (var targetCollider in targetsInViewRadius)
            {
                var targetZombieObject = targetCollider.gameObject;
                var targetZombie = targetZombieObject.GetComponent<Zombie>();
                targetZombie.SetZombieState(ZombieState.ALARMED);
                targetZombie.SetAgentDestination(transform.position);
            }
            Destroy(this);
        }
    }

	void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	void OnCollision(Collision collision)
	{
		_source.PlayOneShot(_source.clip);
	}
}