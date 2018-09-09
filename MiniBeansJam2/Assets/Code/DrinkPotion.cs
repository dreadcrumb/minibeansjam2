using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkPotion : MonoBehaviour
{

	private AudioSource _source;

	// Use this for initialization
	void Start ()
	{
		_source = GetComponent<AudioSource>();
	}

	public void Drink()
	{
		_source.PlayOneShot(_source.clip);
	}
}
