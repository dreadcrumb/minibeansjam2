using UnityEngine;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            // TODO play trap sound
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}