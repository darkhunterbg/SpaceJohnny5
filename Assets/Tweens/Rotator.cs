using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 RotateAxis;

	public float Speed = 10f;

	private float _degrees = 0;

	// Update is called once per frame
	void Update()
	{
		_degrees += Time.deltaTime * Speed;
		_degrees = (_degrees % 360 + 360) % 360;

		transform.localRotation = Quaternion.Euler(RotateAxis.x * _degrees, RotateAxis.y * _degrees, 0);
	}
}
