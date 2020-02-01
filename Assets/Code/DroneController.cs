using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
	public Camera Camera;
	public GameObject MovementActivateObject;
	public GameObject ShipVisual;

	[Header("Camera Settings")]

	public float Distance;
	public float VerticalShift;

	[Header("Drone Settings")]
	public float MaxVelocity = 5;
	public float Acceleration = 1f;
	public float NoInputAcceleration = -5f;


	public AnimationCurve MouseInputRotationRateCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float MouseInputMaxRotationRate = 100;

	public AnimationCurve NoInputRotationRateCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float NoInputRotationRateCurveSize = 10;
	public float NoInputRotationRate = -100;

	[Header("Stats")]
	public float Velocity;
	public Vector2 RotationRate;

	// Start is called before the first frame update
	void Start()
	{

	}

	Vector2 NormalizedMousePos()
	{
		return (Input.mousePosition / new Vector2(Screen.width, Screen.height));
	}

	// Update is called once per frame
	void Update()
	{
		bool input = Input.GetMouseButton(0);

		MovementActivateObject.SetActive(input);


		if (input) {
			Vector2 mousePosLinear = NormalizedMousePos() * 2.0f - Vector2.one;
			float actualLength = MouseInputRotationRateCurve.Evaluate(mousePosLinear.magnitude) * MouseInputMaxRotationRate;

			Vector2 rotAccel = mousePosLinear.normalized * actualLength;
			RotationRate = new Vector2(-rotAccel.y, rotAccel.x) * Time.deltaTime;

			//	ShipVisual.transform.localRotation = Quaternion.identity;
			//	ShipVisual.transform.Rotate(new Vector3(-mouseOffset.y * 30, mouseOffset.x * 30, 0));
		} else {
			float s = RotationRate.magnitude / NoInputRotationRateCurveSize;
			float rr = NoInputRotationRateCurve.Evaluate(s) * NoInputRotationRate;

			float length = Mathf.Max(0, RotationRate.magnitude + rr * Time.deltaTime);
			RotationRate = RotationRate.normalized * length;
			if (RotationRate.magnitude < 1.0f)
				RotationRate = Vector2.zero;
		}

		if (RotationRate != Vector2.zero) {
			Vector3 rot = new Vector3(RotationRate.x, RotationRate.y, 0);
			rot *= Time.deltaTime;
			transform.Rotate(rot);
		}

		float accel = input ? Acceleration : NoInputAcceleration;
		Velocity = Mathf.Clamp(Velocity + accel * Time.deltaTime, 0, MaxVelocity);
		transform.position += Velocity * Time.deltaTime * transform.forward;
	}

	private void LateUpdate()
	{
		bool input = Input.GetMouseButton(0);
		Vector2 mouseOffset = Vector2.zero;
		if (input) {
			Vector2 sceenCenter = new Vector2(Screen.width, Screen.height) / 2;
			Vector2 mousePos = Input.mousePosition;
			mouseOffset = mousePos - sceenCenter;
			mouseOffset /= 200.0f;
		}

		Vector3 offset = Vector3.zero;
		offset += -transform.forward * Distance;
		offset += transform.up * VerticalShift;
		Camera.transform.position = transform.position + offset;
		Camera.transform.rotation = transform.rotation;
		//Camera.transform.Rotate(new Vector3(-mouseOffset.y * 10, mouseOffset.x * 10, 0));
	}
}
