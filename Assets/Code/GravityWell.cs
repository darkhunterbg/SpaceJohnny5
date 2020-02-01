using UnityEngine;

public class GravityWell : MonoBehaviour
{
	public float GravityStrength = 10;

	[Tooltip("Leave zero to pull to gravity well's center")]
	public Vector3 Direction;

	private DroneController _droneInside;

	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
		Debug.Assert(_collider != null);
		FindObjectOfType<GameLevel>().GravityWells.Add(this);
	}

	public bool TestInside(Bounds bounds)
	{
		return _collider.bounds.Intersects(bounds);
	}

	public bool TestInside(Vector2 point)
	{
		return _collider.bounds.Contains(point);
	}

	public void Update()
	{
		bool inside = _droneInside != null; 

		if (inside) {

			Vector3 dir = Direction == Vector3.zero ? (_droneInside.transform.position - transform.position).normalized : Direction.normalized;

			_droneInside.Velocity -= dir * GravityStrength * Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		_droneInside = other.GetComponent<DroneController>() ?? _droneInside;
	}

	private void OnTriggerExit(Collider other)
	{
		if (_droneInside!=null && other.gameObject == _droneInside.gameObject)
			_droneInside = null;
	}
}
