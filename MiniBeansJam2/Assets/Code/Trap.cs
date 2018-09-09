using UnityEngine;

public class Trap : MonoBehaviour
{
	private AudioSource _source;

	void Start()
	{
		_source = GetComponent<AudioSource>();
		_source.PlayOneShot(_source.clip);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Target"))
		{
			_source.PlayOneShot(_source.clip);
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}
}