using UnityEngine;

public class Oscillator : MonoBehaviour
{
	public Vector3 OscillateAxis;
	public float Angle;
	public float OscillateRotateSpeed;

	public bool OscillatePosition;
	public float OscillatePositionSpeed;
	public float PositionMult;
	public Vector3 PositionOffset;

	// Update is called once per frame
	void Update()
    {
		if (OscillatePosition) {
			transform.localPosition = PositionOffset + new Vector3(transform.localPosition.x,
				transform.localPosition.y,
				Mathf.Sin(Time.time * OscillatePositionSpeed) * PositionMult * OscillateAxis.z);
		} else {
			transform.localRotation = Quaternion.Euler(OscillateAxis.x * Angle * Mathf.Sin(Time.time * OscillateRotateSpeed), 
				OscillateAxis.y * Angle * Mathf.Cos(Time.time * OscillateRotateSpeed), 0);
		}
	}
}
