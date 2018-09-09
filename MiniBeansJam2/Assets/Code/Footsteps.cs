using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour {

	private AudioSource _source;

	// Use this for initialization
	void Start () {
		_source = GetComponent<AudioSource>();
		_source.volume = 0.3f;
	}

	public void SetFootstepsPlaying(bool playing)
	{
		if (playing)
		{
			if (!_source.isPlaying)
			_source.Play();
		}
		else
		{
			_source.Stop();
		}
	}
}
