using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionMenu : MonoBehaviour
{
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
    }

    public void OnTrapPlaceClick()
    {
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
        Player selected = GetSelectedPlayer();
        if (selected.Items[ItemType.STONES] > 0)
        {
            Debug.Log(gameObject.transform.position.y);
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