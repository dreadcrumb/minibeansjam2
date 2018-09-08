using System.Collections.Generic;
using UnityEngine;

public class HazardArea : MonoBehaviour
{
    public double ZombficationIncrementOnTouch = 0.01;
    public bool ReapeatedTouch = false;

    private HashSet<GameObject> _alreadyTouched = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        var targetObject = other.gameObject;
        if (targetObject.CompareTag("Player"))
        {
            if (ReapeatedTouch || !_alreadyTouched.Contains(targetObject))
            {
                var player = targetObject.GetComponent<Player>();
                player.ZombificationPassiveIncrement += ZombficationIncrementOnTouch;
                _alreadyTouched.Add(targetObject);
            }
        }
    }
}