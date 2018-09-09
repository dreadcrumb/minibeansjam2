using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpdater : MonoBehaviour
{
	public GameObject HealthCanvas;
	public GameObject ZombificationCanvas;
	public GameObject StonesTextfield;
	public GameObject TrapsTextfield;
	public GameObject PillsTextfield;
	public GameObject TankCanvasGroup;
	public GameObject ArcherCanvasGroup;
	public GameObject AlchemistCanvasGroup;

	void Update()
	{
		var player = GetSelectedPlayer();
		if (player == null)
		{
			return;
		}

		switch (player.Type)
		{
			case PlayerType.MAGE:
				HideTankUI();
				HideArcherUI();
				ShowAlchemistUI();
				break;
			case PlayerType.TANK:
				ShowTankUI();
				HideArcherUI();
				HideAlchemistUI();
				break;
			case PlayerType.ARCHER:
				HideTankUI();
				HideAlchemistUI();
				ShowArcherUI();
				break;
		}

		HealthCanvas.GetComponent<Image>().fillAmount = (float) (player.Health / 100);
		if (player.ZombificationLevel > 0)
		{
			ZombificationCanvas.GetComponent<Image>().fillAmount = (float) (player.ZombificationLevel / 100);
		}
		else
		{
			ZombificationCanvas.GetComponent<Image>().fillAmount = 0;
		}

		int firstItemAmount;
		switch (player.Type)
		{
			case PlayerType.MAGE:
			case PlayerType.TANK: 
				firstItemAmount = player.Items[ItemType.STONES];
				break;
			case PlayerType.ARCHER:
				firstItemAmount = player.Items[ItemType.ARROW];
				break;
			default:
				firstItemAmount = 0;
				break;
		}
		
		int secondItemAmount;
		switch (player.Type)
		{
			case PlayerType.MAGE:
				secondItemAmount = player.Items[ItemType.EXPLOSIVE];
				break;
			case PlayerType.TANK: 
			case PlayerType.ARCHER:
				secondItemAmount = player.Items[ItemType.TRAPS];
				break;
			default:
				secondItemAmount = 0;
				break;
		}

		StonesTextfield.GetComponent<Text>().text = firstItemAmount.ToString();
		TrapsTextfield.GetComponent<Text>().text = secondItemAmount.ToString();
		PillsTextfield.GetComponent<Text>().text = player.Items[ItemType.PILLS].ToString();
	}

	private void HideTankUI()
	{
		HideGroup(TankCanvasGroup.GetComponent<CanvasGroup>());
	}

	private void HideAlchemistUI()
	{
		HideGroup(AlchemistCanvasGroup.GetComponent<CanvasGroup>());
	}

	private void HideArcherUI()
	{
		HideGroup(ArcherCanvasGroup.GetComponent<CanvasGroup>());
	}

	private void ShowTankUI()
	{
		ShowGroup(TankCanvasGroup.GetComponent<CanvasGroup>());
	}

	private void ShowAlchemistUI()
	{
		ShowGroup(AlchemistCanvasGroup.GetComponent<CanvasGroup>());
	}

	private void ShowArcherUI()
	{
		ShowGroup(ArcherCanvasGroup.GetComponent<CanvasGroup>());
	}


	private void HideGroup(CanvasGroup group)
	{
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
	}

	private void ShowGroup(CanvasGroup group)
	{
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
	}

	private Player GetSelectedPlayer()
	{
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