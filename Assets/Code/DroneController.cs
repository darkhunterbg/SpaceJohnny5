using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
	public Camera Camera;
	public GameObject MovementActivateObject;

	[Header("Camera Settings")]

	public float Distance;
	public float VerticalShift;

	[Header("Drone Settings")]
	public float MaxVelocity = 5;
	public float Acceleration = 1f;
	public float NoInputAcceleration = -5f;

	[Header("Stats")]
	public float Velocity;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		bool input = Input.GetMouseButton(0);

		MovementActivateObject.SetActive(input);

		float accel = input ? Acceleration : NoInputAcceleration;

		Velocity = Mathf.Clamp(Velocity + accel * Time.deltaTime, 0, MaxVelocity);

		transform.position += Velocity * Time.deltaTime * transform.forward;
	}

	private void LateUpdate()
	{
		bool input = Input.GetMouseButton(0);

		if (input) {
			Vector2 sceenCenter = new Vector2(Screen.width, Screen.height) / 2;
			Vector2 mousePos = Input.mousePosition;
			Vector2 mouseOffset = mousePos - sceenCenter;

			mouseOffset /= 200.0f;

			transform.Rotate(new Vector3(-mouseOffset.y, mouseOffset.x, 0));
		}

		Vector3 offset = Vector3.zero;
		offset += -transform.forward * Distance;
		offset += transform.up * VerticalShift;
		Camera.transform.position = transform.position + offset;
		Camera.transform.localRotation = transform.rotation;
		//Camera.transform.forward = transform.forward;
		//Camera.transform.up = transform.up;
		//Camera.transform.right = transform.right;
	}
}
