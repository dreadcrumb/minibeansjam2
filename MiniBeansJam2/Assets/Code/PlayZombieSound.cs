using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZombieSound : MonoBehaviour
{

	public float MaxHearingDistance = 25;
	private AudioSource _source;
	private float _startVolume = 0.4f;

	// Use this for initialization
	void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetZombiesVolume()
	{
		int count = 0;
		foreach (Zombie e in FindObjectsOfType<Zombie>())
		{
			float dist = Vector3.Distance(e.transform.position, transform.position);
			if (dist < MaxHearingDistance)
			{
				if (!_source.isPlaying)
				{
					_source.Play();
				}
				_source.volume = _startVolume - dist / MaxHearingDistance * _startVolume;
			}
			count++;
		}

		if (count == 0)
		{
			_source.Stop();
		}

	}
}
