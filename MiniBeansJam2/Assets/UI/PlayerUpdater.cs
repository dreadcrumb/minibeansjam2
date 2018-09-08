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

    void Update()
    {
        var player = GetSelectedPlayer();
        if (player == null)
        {
            return;
        }
        
        HealthCanvas.GetComponent<Image>().fillAmount = (float) (player.Health / 100);
        if (player.ZombificationLevel > 0)
        {
            ZombificationCanvas.GetComponent<Image>().fillAmount = (float)(player.ZombificationLevel / 100);
        }
        else
        {
            ZombificationCanvas.GetComponent<Image>().fillAmount = 0;
        }

        StonesTextfield.GetComponent<Text>().text = player.Items[ItemType.STONES].ToString();
        TrapsTextfield.GetComponent<Text>().text = player.Items[ItemType.TRAPS].ToString();
        PillsTextfield.GetComponent<Text>().text = player.Items[ItemType.PILLS].ToString();
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