using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float Speed = 0.25f;
    public int EdgeSize = 10;
    
    void FixedUpdate()
    {
        var edgeX = Screen.width / EdgeSize;
        var edgeY = Screen.height / EdgeSize;
        
        var mousePosition = Input.mousePosition;
        Vector3 movement = Vector3.zero;
        if (mousePosition.x < edgeX)
        {
            movement += Vector3.left;
        }
        else if (mousePosition.x > Screen.width - edgeX)
        {
            movement += Vector3.right;
        }

        if (mousePosition.y < edgeY)
        {
            movement += Vector3.back;
        }
        else if (mousePosition.y > Screen.height - edgeX)
        {
            movement += Vector3.forward;
        }

        transform.position += movement * Speed;
    }
}