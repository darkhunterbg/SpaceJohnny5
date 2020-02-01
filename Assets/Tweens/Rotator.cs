using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 RotateAxis;

	public float Speed = 10f;

	private float _degrees = 0;

	public bool RandomizeSpeed;
	public int MinDegree;
	public int MaxDegree;

	public float CurrentRandeomDegree;

	// Update is called once per frame
	void Update()
	{
		if (RandomizeSpeed) {
			CurrentRandeomDegree = Random.Range(MinDegree, MaxDegree) * Speed;
			_degrees += CurrentRandeomDegree;
		} else {
			_degrees += Time.deltaTime * Speed;
		}
		_degrees = (_degrees % 360 + 360) % 360;

		transform.localRotation = Quaternion.Euler(RotateAxis.x * _degrees, RotateAxis.y * _degrees, 0);
	}
}
