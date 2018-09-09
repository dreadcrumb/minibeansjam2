using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionMenu : MonoBehaviour
{
	private AudioSource _source;

	void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
		{
			Debug.Log("Deselecting menu.");
			gameObject.SetActive(false);
		}
	}

	public void OnMoveClick()
	{
		Player selected = GetSelectedPlayer();
		selected.MoveTo(transform.position);
		gameObject.SetActive(false);
		_source.Play();
	}

	public void OnTrapPlaceClick()
	{
		_source.Play();
		Player selected = GetSelectedPlayer();
		if (selected.Items[ItemType.TRAPS] > 0)
		{
			selected.PlaceTrapAtIfInRange(transform.position);
			gameObject.SetActive(false);
		}
		else
		{
			Debug.Log("Not enough traps!");
		}
	}

	public void OnTakePillClicked()
	{
		_source.Play();
		Player selected = GetSelectedPlayer();
		if (selected.Items[ItemType.PILLS] > 0)
		{
			selected.TakePill();
			gameObject.SetActive(false);
		}
		else
		{
			Debug.Log("Not enough pills!");
		}
	}

	public void OnThrowStoneClicked()
	{
		_source.Play();
		Player selected = GetSelectedPlayer();
		if (selected.Items[ItemType.STONES] > 0)
		{
			selected.ThrowStoneIfInRange(gameObject.transform.position);
			gameObject.SetActive(false);
		}
		else
		{
			Debug.Log("Not enough stones!");
		}
	}

	private Player GetSelectedPlayer()
	{
		_source.Play();
		foreach (var playerObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			var player = playerObject.GetComponent<Player>();
			if (player.IsSelected())
			{
				return player;
			}
		}

		return null;
	}
}