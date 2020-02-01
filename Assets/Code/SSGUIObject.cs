using UnityEngine;

public class SSGUIObject : MonoBehaviour
{
	public GameObject TrackingObject;


	public void Update()
	{
		if (TrackingObject == null)
			return;

		var camera = Camera.main;
		Vector3 distance = TrackingObject.transform.position + camera.transform.forward - camera.transform.position;
	}
}
