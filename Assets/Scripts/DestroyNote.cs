using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Scripts;
using UnityEngine;

public class DestroyNote : MonoBehaviour
{

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag(Const.Tags.Note))
		{
			Destroy(collision.gameObject);
		}
	}
}
