using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	public float Speed = 0.25f;
	public float RotationSpeed = 10f;
	public int EdgeSize = 10;
	public int MaxZoom = 10;
	public int MinZoom = 20;

	void FixedUpdate()
	{
		var edgeX = Screen.width / EdgeSize;
		var edgeY = Screen.height / EdgeSize;

		var mousePosition = Input.mousePosition;
		Vector3 movement = Vector3.zero;
		if (mousePosition.x < edgeX)
		{
			movement -= transform.right;
		}
		else if (mousePosition.x > Screen.width - edgeX)
		{
			movement += transform.right;
		}

		if (mousePosition.y < edgeY)
		{
			Vector3 forwardVector = transform.forward;
			forwardVector.y = 0;
			forwardVector.Normalize();
			movement -= forwardVector;
		}
		else if (mousePosition.y > Screen.height - edgeX)
		{
			Vector3 forwardVector = transform.forward;
			forwardVector.y = 0;
			forwardVector.Normalize();
			movement += forwardVector;
		}

		transform.position += movement * Speed;

		var scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll < 0)
		{
			transform.position = new Vector3(transform.position.x, Math.Min(MinZoom, transform.position.y + 1), transform.position.z);
		}
		else if (scroll > 0)
		{
			transform.position = new Vector3(transform.position.x, Math.Max(MaxZoom, transform.position.y - 1), transform.position.z);
		}

		if (Input.GetKey(KeyCode.A))
		{
			//Camera mainCamera = Camera.main;
			//mainCamera.
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - RotationSpeed, transform.eulerAngles.z);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			//Camera mainCamera = Camera.main;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + RotationSpeed, transform.eulerAngles.z);
		}
	}
}