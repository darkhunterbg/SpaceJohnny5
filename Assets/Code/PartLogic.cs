using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PartLogic : MonoBehaviour
{
	public float Deceleration = 3;
	public float FollowSpeed = 3;
	public float RotateSpeed = 3;
	public float FollowDistance = 2.5f;
	
	private Transform _followTarget;
	private PartLogic _nextPart;
	private Vector3 _velocity;
	private ShipLogic _shipToAttachTo;

	public bool AttachedToAnything => _followTarget != null;
	
	public void Attach(Transform followTarget)
	{
		_followTarget = followTarget;
		var prevPart = followTarget.GetComponent<PartLogic>();
		
		if (prevPart != null) {
			prevPart._nextPart = this;
		}

		var ship = followTarget.GetComponent<ShipLogic>();
		
		if (ship != null) {
			_shipToAttachTo = ship;
		}
	}

	public void Detach()
	{
		_followTarget = null;
		_nextPart = null;
	}

	private void Update()
	{
		if (_followTarget != null) {
			Vector3 newPos;
			
			if (_shipToAttachTo != null) {
				newPos = Vector3.Lerp(transform.position, _followTarget.position, Time.deltaTime * FollowSpeed);
			} else {
				Vector3 dist = transform.position - _followTarget.position;
				Vector3 followPoint = _followTarget.position + dist.normalized * FollowDistance;
				newPos = Vector3.Lerp(transform.position, followPoint, Time.deltaTime * FollowSpeed);
			}

			_velocity = transform.position - newPos;
			transform.position = newPos;
			Vector3 lookAt = _followTarget.position - transform.position;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAt), Time.deltaTime * RotateSpeed);

			if (_shipToAttachTo != null && (_followTarget.position - transform.position).magnitude < 1) {
				_shipToAttachTo.DeliverPart(this);
				
				if (_nextPart != null) {
					_nextPart.Attach(_shipToAttachTo.transform);
				}
				
				Destroy(gameObject);
			}
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
