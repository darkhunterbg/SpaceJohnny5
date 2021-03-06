using UnityEngine;

public enum DroneControllerState
{
	FreeControl,
	Hit,
	Dead,
}

public class DroneController : MonoBehaviour
{
	public Camera Camera;
	public ParticleSystem LeftThruster;
	public ParticleSystem[] LeftThrusterSecondary;
	public TrailRenderer[] LeftTrails;
	public ParticleSystem RightThruster;
	public ParticleSystem[] RightThrusterSecondary;
	public TrailRenderer[] RightTrails;
	public GameObject ShipVisual;
	public AudioSource ThrustersAudio;

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
	public float BoostVelocityMultiplier = 3;
	public float ForwardAcceleration = 1f;
	public float BackwardAcceleration = 1f;
	public float StrafeAcceleration = 1f;
	public float NoInputDampening = 0.1f;

	public float BoostedMaxVelocity => MaxVelocity * BoostVelocityMultiplier;

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
	public bool ThrustersActive;

	private float VelocityChangedUntilTime;


	private float MaxVelocityIncludingMultiplier
	{
		get
		{
			if (Time.time < VelocityChangedUntilTime) {
				return MaxVelocity * BoostVelocityMultiplier;
			}

			return MaxVelocity;
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// private void OnGUI()
	// {
	// 	if(GUI.Button(new Rect(0,0,100,20), "Hit Test")) {
	// 		ApplyHit(new Vector3(100, 0,0));
	// 	}
	// }

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
		if (State == DroneControllerState.Hit && Velocity.magnitude == 0) {
			State = DroneControllerState.FreeControl;
		}

		float deltaT;
		Vector3 Acceleration = Vector3.zero;

		bool mouseInput = false;

		deltaT = Time.deltaTime;

			mouseInput = ProcessInput(deltaT, ref Acceleration);
		

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

		bool dampen = State == DroneControllerState.Hit || Acceleration == Vector3.zero || VelocityMagnitude > MaxVelocityIncludingMultiplier;

		if (dampen) {
			Acceleration = (-Velocity * NoInputDampening);
		} else {

			Acceleration = Acceleration.normalized * Mathf.Clamp(Acceleration.magnitude, 0, ForwardAcceleration);
		}

		AccelerationMagnitude = Acceleration.magnitude;


		Velocity += Acceleration * deltaT;
		VelocityMagnitude = Velocity.magnitude;


		if (!dampen ) {

			VelocityMagnitude = Mathf.Clamp(Velocity.magnitude, 0, MaxVelocityIncludingMultiplier);
		}

		if (VelocityMagnitude < 0.01f)
			VelocityMagnitude = 0;

		Velocity = Velocity.normalized * VelocityMagnitude;

		PrevPosition = transform.position;

		transform.position += Velocity * deltaT;

		if (ThrustersActive && !ThrustersAudio.isPlaying) {
			ThrustersAudio.Play();
		} else if (!ThrustersActive && ThrustersAudio.isPlaying) {
			ThrustersAudio.Stop();
		}

		if (MaxVelocityIncludingMultiplier != MaxVelocity) {
			ThrustersAudio.pitch = 1.2f;
		} else {
			ThrustersAudio.pitch = 1.0f;
		}
	}

	private bool ProcessInput(float deltaT, ref Vector3 Acceleration)
	{
		bool input = false;

		if (State == DroneControllerState.FreeControl) {
			input = Input.GetMouseButton(0);

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

		}
		bool leftState = false;
		bool rightState = false;
		ThrustersActive = false;

		if (State == DroneControllerState.FreeControl) {
			Acceleration = Vector3.zero;
			if (Input.GetKey(KeyCode.W)) {
				Acceleration += transform.forward * ForwardAcceleration;
				leftState = true;
				rightState = true;
				ThrustersActive = true;
			}
			if (Input.GetKey(KeyCode.S)) {
				Acceleration -= transform.forward * BackwardAcceleration;
				leftState = true;
				rightState = true;
				ThrustersActive = true;
			}
			if (Input.GetKey(KeyCode.A)) {
				Acceleration -= transform.right * StrafeAcceleration;
				rightState = true;
				ThrustersActive = true;
			}
			if (Input.GetKey(KeyCode.D)) {
				Acceleration += transform.right * StrafeAcceleration;
				leftState = true;
				ThrustersActive = true;
			}
		}

#pragma warning disable 0618
		if (leftState && !LeftThruster.enableEmission)
			LeftThruster.enableEmission = true;

		if (!leftState && LeftThruster.enableEmission)
			LeftThruster.enableEmission = false;

		if (rightState && !RightThruster.enableEmission)
			RightThruster.enableEmission = true;

		if (!rightState && RightThruster.enableEmission)
			RightThruster.enableEmission = false;

		foreach (TrailRenderer t in LeftTrails) {
			t.emitting = LeftThruster.enableEmission;
		}

		foreach (var secondary in LeftThrusterSecondary) {
			secondary.enableEmission = LeftThruster.enableEmission;
		}

		foreach (TrailRenderer t in RightTrails) {
			t.emitting = RightThruster.enableEmission;
		}

		foreach (var secondary in RightThrusterSecondary) {
			secondary.enableEmission = RightThruster.enableEmission;
		}
#pragma warning restore 0618

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

	public void IncreaseVelocityFor(float time)
	{
		VelocityChangedUntilTime = Time.time + time;
	}
}
