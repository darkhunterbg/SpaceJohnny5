using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PartLogic : MonoBehaviour
{
	public float Deceleration = 3;
	public float FollowSpeed = 3;
	public float RotateSpeed = 3;
	public float FollowDistance = 2.5f;
	
	private Transform _followTarget;
	private Vector3 _velocity;
	
	public void Attach(Transform followTarget)
	{
		_followTarget = followTarget;
	}

	public void Detach()
	{
		_followTarget = null;
	}

	private void Update()
	{
		if (_followTarget != null) {
			Vector3 dist = transform.position - _followTarget.position;
			Vector3 followPoint = _followTarget.position + dist.normalized * FollowDistance;
			Vector3 newPos = Vector3.Lerp(transform.position, followPoint, Time.deltaTime * FollowSpeed);
			_velocity = transform.position - newPos;
			transform.position = newPos;
			Vector3 lookAt = _followTarget.position - transform.position;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAt), Time.deltaTime * RotateSpeed);
		} else {
			// Decelerate if detached
			float newSpeed = _velocity.magnitude - Time.deltaTime * Deceleration;
			
			if (newSpeed < 0) {
				newSpeed = 0;
			}

			Vector3 newVelocity = _velocity.normalized * newSpeed;
			transform.position += newVelocity * Time.deltaTime;
		}
	}
}
