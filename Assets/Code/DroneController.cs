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
	public Vector2 RotationRateOffsetScale = Vector2.one * 0.1f;
	public float MaxVelocityDistanceShift = 1;
	public AnimationCurve MaxVelocityDistanceShiftCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float VeriticalShiftMaxRotationRateReducing = 1;

	[Header("Input")]
	public float MouseMovementDeadZone = 0.01f;
	public float MouseMovementMaxZone = 0.9f;

	[Header("Drone Settings")]
	public float MaxVelocity = 5;
	public float Acceleration = 1f;
	public float NoInputDampening = 0.1f;

	//public AnimationCurve MouseInputRotationRateCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float MaxRotationRate = 100;
	public float RotationRateAccel = 50;

	public AnimationCurve NoInputRotationRateDampeningCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public float NoInputRotationRateDampeningTime = 2;

	public float RotationRateMouseInputAccelMultiplier = 1;

	[Header("Other")]
	public Vector2 RotationRateExtraVisualRotateScale = Vector2.one * 0.1f;

	[Header("Stats")]
	public float Velocity;

	[HideInInspector]
	public Vector2 RotationRate;
	public float RotationRateMagnitude;

	// Start is called before the first frame update
	void Start()
	{

	}

	Vector2 GetMouseMovement()
	{
		Vector2 mousePos = (Input.mousePosition / new Vector2(Camera.pixelWidth, Camera.pixelHeight));
		Vector2 mousePosLinear = mousePos * 2.0f - Vector2.one;

		float d = MouseMovementMaxZone - MouseMovementDeadZone;

		float l = Mathf.Clamp(mousePosLinear.magnitude - MouseMovementDeadZone, 0, d);

		mousePosLinear = mousePosLinear.normalized * Mathf.Lerp(0, 1, l / d);

		return mousePosLinear;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		ProcessOrientation(input);

		float accel = input ? Acceleration : (-Velocity * NoInputDampening);
		Velocity = Mathf.Clamp(Velocity + accel * Time.fixedDeltaTime, 0, MaxVelocity);
		if (Velocity < 0.01f)
			Velocity = 0;
		transform.position += Velocity * Time.fixedDeltaTime * transform.forward;
	}

	float noInputTime = 0;
	float noInputRotationRateRef;
	bool input = false;

	private void ProcessOrientation(bool input)
	{
		if (input) {
			Vector2 mouseMov = GetMouseMovement();
			mouseMov = new Vector2(-mouseMov.y, mouseMov.x);

			Vector2 desiredRotationRate = mouseMov * MaxRotationRate;
			Vector2 diff = desiredRotationRate - RotationRate;

			if (diff.magnitude < RotationRateAccel * Time.fixedDeltaTime)
				RotationRate = desiredRotationRate;
			else
				RotationRate += diff.normalized * RotationRateAccel * Time.fixedDeltaTime;

			float length = RotationRate.magnitude;
			length = Mathf.Clamp(RotationRate.magnitude, 0, MaxRotationRate);
			RotationRate = RotationRate.normalized * length;
		} else {
			RotationRate = (1.0f - NoInputRotationRateDampeningCurve.Evaluate(noInputTime / NoInputRotationRateDampeningTime)) * noInputRotationRateRef
				* RotationRate.normalized;
		}


		if (RotationRate.magnitude < 0.1f)
			RotationRate = Vector2.zero;

		RotationRateMagnitude = RotationRate.magnitude;

		if (RotationRate != Vector2.zero) {
			Vector3 rot = new Vector3(RotationRate.x, RotationRate.y, 0);
			rot *= Time.fixedDeltaTime;
			transform.Rotate(rot);
		}

		ShipVisual.transform.localRotation = Quaternion.Euler(new Vector3(RotationRate.x * RotationRateExtraVisualRotateScale.y, RotationRate.y * RotationRateExtraVisualRotateScale.x, 0));
	}



	private void Update()
	{
		input = Input.GetMouseButton(0);

		MovementActivateObject.SetActive(input);

		if (input)
			noInputTime = 0;
		else
			noInputTime += Time.deltaTime;

		if (Input.GetMouseButtonUp(0))
			noInputRotationRateRef = RotationRate.magnitude;

		Vector3 offset = Vector3.zero;
		float extraDistanceShift = MaxVelocityDistanceShiftCurve.Evaluate(Velocity / MaxVelocity) * MaxVelocityDistanceShift;
		float distance = Distance + extraDistanceShift;
		offset += -transform.forward * distance;

		float verticalShift = VerticalShift * (1.0f - VeriticalShiftMaxRotationRateReducing * MaxVelocityDistanceShiftCurve.Evaluate(RotationRateMagnitude / MaxRotationRate));

		offset += transform.up * verticalShift;
		Camera.transform.position = transform.position + offset;
		Camera.transform.rotation = transform.rotation;

		// TODO: Dampening/Curve for rotation vertor shifting
		Camera.transform.Rotate(new Vector3(RotationRate.x * RotationRateOffsetScale.y, RotationRate.y * RotationRateOffsetScale.x, 0));
	}
}
