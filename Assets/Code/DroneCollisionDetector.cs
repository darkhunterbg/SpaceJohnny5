using UnityEngine;

namespace DefaultNamespace
{
	public class DroneCollisionDetector : MonoBehaviour
	{
		private DroneLogic _droneLogic;
		
		private void Start()
		{
			_droneLogic = GetComponentInParent<DroneLogic>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			_droneLogic.OnCollisionEnter(collision);
		}
	}
}
