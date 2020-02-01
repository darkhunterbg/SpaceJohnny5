using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneControllerState
{
	FreeControl,
	Hit
}

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
	public float ForwardAcceleration = 1f;
	public float BackwardAcceleration = 1f;
	public float StrafeAcceleration = 1f;
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
	[HideInInspector]
	public Vector3 Velocity;
	public float VelocityMagnitude;

	[HideInInspector]
	public Vector2 RotationRate;
	public float RotationRateMagnitude;

	[HideInInspector]
	public Vector3 Acceleration;
	public float AccelerationMagnitude;

	public DroneControllerState State;

	public Vector3 PrevPosition;

	// Start is called before the first frame update
	void Start()
	{

	}

	private void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,20), "Hit Test")) {
			ApplyHit(new Vector3(100, 0,0));
		}
	}

	public void ApplyHit(Vector3 impulse)
	{
		State = DroneControllerState.Hit;
		Velocity += impulse;
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
	void Update()
	{
		if(State== DroneControllerState.Hit && Velocity.magnitude == 0) {
			State = DroneControllerState.FreeControl;
		}

		float deltaT;
		Vector3 Acceleration = Vector3.zero;

		bool mouseInput = false;

		deltaT = Time.deltaTime;

		if (State == DroneControllerState.FreeControl) {
			mouseInput = ProcessInput(deltaT, ref Acceleration);
		}

		if (!mouseInput) {
			RotationRate = (1.0f - NoInputRotationRateDampeningCurve.Evaluate(noInputTime / NoInputRotationRateDampeningTime)) * noInputRotationRateRef * RotationRate.normalized;
		}
		if (RotationRate.magnitude < 0.1f)
			RotationRate = Vector2.zero;

		RotationRateMagnitude = RotationRate.magnitude;

		if (RotationRate != Vector2.zero) {
			Vector3 rot = new Vector3(RotationRate.x, RotationRate.y, 0);
			rot *= deltaT;
			transform.Rotate(rot);
		}

		ShipVisual.transform.localRotation = Quaternion.Euler(new Vector3(RotationRate.x * RotationRateExtraVisualRotateScale.y, RotationRate.y * RotationRateExtraVisualRotateScale.x, 0));



		if (Acceleration == Vector3.zero) {
			Acceleration = (-Velocity * NoInputDampening);
			MovementActivateObject.SetActive(false);
		} else {
			Acceleration = Acceleration.normalized * Mathf.Clamp(Acceleration.magnitude, 0, ForwardAcceleration);
			MovementActivateObject.SetActive(true);
		}

		AccelerationMagnitude = Acceleration.magnitude;

		Velocity += Acceleration * deltaT;
		VelocityMagnitude = Velocity.magnitude;

		if (State == DroneControllerState.FreeControl) {
			VelocityMagnitude = Mathf.Clamp(Velocity.magnitude, 0, MaxVelocity);
		}

		if (VelocityMagnitude < 0.01f)
			VelocityMagnitude = 0;

		Velocity = Velocity.normalized * VelocityMagnitude;

		PrevPosition = transform.position;

		transform.position += Velocity * deltaT;
	}

	private bool ProcessInput(float deltaT, ref Vector3 Acceleration)
	{
		bool input = Input.GetMouseButton(0);

		if (input)
			noInputTime = 0;
		else
			noInputTime += Time.deltaTime;

		if (input)
			noInputRotationRateRef = RotationRate.magnitude;

		if (input) {
			Vector2 mouseMov = GetMouseMovement();
			mouseMov = new Vector2(-mouseMov.y, mouseMov.x);

			Vector2 desiredRotationRate = mouseMov * MaxRotationRate;
			Vector2 diff = desiredRotationRate - RotationRate;

			if (diff.magnitude < RotationRateAccel * Time.fixedDeltaTime)
				RotationRate = desiredRotationRate;
			else
				RotationRate += diff.normalized * RotationRateAccel * deltaT;

			float length = RotationRate.magnitude;
			length = Mathf.Clamp(RotationRate.magnitude, 0, MaxRotationRate);
			RotationRate = RotationRate.normalized * length;
		}

		Acceleration = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) {
			Acceleration += transform.forward * ForwardAcceleration;
		}
		if (Input.GetKey(KeyCode.S)) {
			Acceleration -= transform.forward * BackwardAcceleration;
		}
		if (Input.GetKey(KeyCode.A)) {
			Acceleration -= transform.right * StrafeAcceleration;
		}
		if (Input.GetKey(KeyCode.D)) {
			Acceleration += transform.right * StrafeAcceleration;
		}


		return input;
	}

	float noInputTime = 0;
	float noInputRotationRateRef;



	Quaternion camRotation = Quaternion.identity;

	private void LateUpdate()
	{
		Vector3 offset = Vector3.zero;
		float extraDistanceShift = MaxVelocityDistanceShiftCurve.Evaluate(VelocityMagnitude / MaxVelocity) * MaxVelocityDistanceShift;
		float distance = Distance + extraDistanceShift;
		offset += -transform.forward * distance;

		float verticalShift = VerticalShift * (1.0f - VeriticalShiftMaxRotationRateReducing * MaxVelocityDistanceShiftCurve.Evaluate(RotationRateMagnitude / MaxRotationRate));

		offset += transform.up * verticalShift;
		Camera.transform.position = transform.position + offset;

		//var targetRotation = transform.rotation;
		//camRotation = Quaternion.Slerp(camRotation, targetRotation, Time.deltaTime * 3);
		//Camera.transform.rotation = camRotation;

		Camera.transform.rotation = transform.rotation;
		Camera.transform.Rotate(new Vector3(RotationRate.x * RotationRateOffsetScale.y, RotationRate.y * RotationRateOffsetScale.x, 0));
	}
}
