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
	public float NoInputDampening = 0.1f;

	//public AnimationCurve MouseInputRotationRateCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float MaxRotationRate = 100;
	public float RotationRateAccel = 50;

	public float NoInputRotationRateDampening = 0.5f;

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
	void FixedUpdate()
	{
		bool input = Input.GetMouseButton(0);

		MovementActivateObject.SetActive(input);

		ProcessOrientation();

		float accel = input ? Acceleration : (-Velocity * NoInputDampening) ;
		Velocity = Mathf.Clamp(Velocity + accel * Time.fixedDeltaTime, 0, MaxVelocity);
		if (Velocity < 0.01f)
			Velocity = 0;
		transform.position += Velocity * Time.fixedDeltaTime * transform.forward;
	}

	private void ProcessOrientation()
	{
		bool input = Input.GetMouseButton(0);

		Vector2 rotAccel = (-RotationRate * NoInputRotationRateDampening);
		float maxRotRate = MaxRotationRate;

		if (input) {
			Vector2 mousePosLinear = NormalizedMousePos() * 2.0f - Vector2.one;

			rotAccel = mousePosLinear.normalized * RotationRateAccel;
			rotAccel = new Vector2(-rotAccel.y, rotAccel.x);

			maxRotRate = Mathf.Min(mousePosLinear.magnitude * MaxRotationRate, MaxRotationRate);
		}
	

		RotationRate += rotAccel * Time.fixedDeltaTime;
		float length = Mathf.Clamp(RotationRate.magnitude, 0, maxRotRate);
		RotationRate = RotationRate.normalized * length;
		if (RotationRate.magnitude < 1.0f)
			RotationRate = Vector2.zero;

		if (RotationRate != Vector2.zero) {
			Vector3 rot = new Vector3(RotationRate.x, RotationRate.y, 0);
			rot *= Time.fixedDeltaTime;
			transform.Rotate(rot);
		}

		//ShipVisual.transform.localRotation = Quaternion.Euler(new Vector3(RotationRate.x, RotationRate.y, 0));
		//ShipVisual.transform.Rotate(new Vector3(-mouseOffset.y * 30, mouseOffset.x * 30, 0));
	}

	private void Update()
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
