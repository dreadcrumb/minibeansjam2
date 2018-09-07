using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private GameObject selectedZombie;
	public const string ZOMBIE_TAG = "Target";

	private List<GameObject> enemyList;

	// Use this for initialization
	void Start()
	{
		enemyList = GameObject.FindGameObjectsWithTag(ZOMBIE_TAG).ToList();
	}

	// Update is called once per frame
	void Update()
	{
		Ray clickRay;
		RaycastHit hit;
		if (Input.GetMouseButtonDown(1))
		{

			clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(clickRay, out hit))
			{
				var colliderGameObject = hit.collider.gameObject;
				if (colliderGameObject.CompareTag(ZOMBIE_TAG))
				{
					ClearSelectedEnemies();
					colliderGameObject.GetComponent<FieldOfView>().SetSelected(true);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			ClearSelectedEnemies();
		}
	}

	private void ClearSelectedEnemies()
	{
		foreach (var enemy in enemyList)
		{
			enemy.GetComponent<FieldOfView>().SetSelected(false);
		}
	}
}
