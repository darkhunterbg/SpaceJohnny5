using UnityEngine;
using UnityEngine.UI;


[DefaultExecutionOrder(200)]
public class SSGUIObject : MonoBehaviour
{
	public GameObject TrackingObject;

	private Canvas _canvas;

	public GameObject OutOfSceenVisuals;
	public GameObject OnScreenVisuals;

	public Text DistanceText;


	public void Init(Canvas canvas)
	{
		this._canvas = canvas;
	}

	public void Update()
	{
		if (TrackingObject == null)
			return;

		var camera = Camera.main;
		Vector3 ssPos = camera.WorldToScreenPoint(TrackingObject.transform.position);

		if (ssPos.z < 0) {
			ssPos *= Vector2.one * -1;
		}

		Vector2 size = ((RectTransform)transform).rect.size;

		bool outOfSceen = ssPos.z < 0 || ssPos.x < 0 || ssPos.x > camera.pixelWidth || ssPos.y < 0 || ssPos.y > camera.pixelHeight;

		if (OutOfSceenVisuals != null)
			OutOfSceenVisuals.SetActive(outOfSceen);

		if (OnScreenVisuals != null)
			OnScreenVisuals.SetActive(!outOfSceen);

		ssPos.x = Mathf.Clamp(ssPos.x, size.x / 2, camera.pixelWidth - size.x / 2);
		ssPos.y = Mathf.Clamp(ssPos.y, size.y / 2, camera.pixelHeight - size.y / 2);

		((RectTransform)transform).position = ssPos;

		if (outOfSceen && OutOfSceenVisuals != null) {
			Vector2 norm = (new Vector2(ssPos.x, ssPos.y) - new Vector2(camera.pixelWidth, camera.pixelHeight) / 2).normalized;

			float rotAngle = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg;// + Mathf.PI  ;
			((RectTransform)OutOfSceenVisuals.transform).rotation = Quaternion.Euler(0, 0, rotAngle - 90);

		} else if (OnScreenVisuals != null) {
			DistanceText.text = (TrackingObject.transform.position - camera.transform.position).magnitude.ToString("0.0").Replace(',', '.');
		}
	}
}
