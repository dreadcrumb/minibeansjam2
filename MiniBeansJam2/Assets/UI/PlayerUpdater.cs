using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpdater : MonoBehaviour
{
    public GameObject HealthCanvas;
    public GameObject ZombificationCanvas;

    void Update()
    {
        var player = GetSelectedPlayer();
        HealthCanvas.GetComponent<Image>().fillAmount = player.Health / 100f;
        if (player.ZombificationLevel > 0)
        {
            ZombificationCanvas.GetComponent<Image>().fillAmount = 100 / (float) player.ZombificationLevel;
        }
        else
        {
            ZombificationCanvas.GetComponent<Image>().fillAmount = 0;
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