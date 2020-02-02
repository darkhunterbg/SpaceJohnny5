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

	public Transform DistanceFrom;

	public void Init(Canvas canvas, Transform from)
	{
		this._canvas = canvas;
		DistanceFrom = from;
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
		Vector2 o = new Vector2(80, 80);

		bool outOfSceen = ssPos.z < 0 || ssPos.x < o.x || ssPos.x > camera.pixelWidth - o.x || ssPos.y < o.y || ssPos.y > camera.pixelHeight - o.y;

		if (OutOfSceenVisuals != null)
			OutOfSceenVisuals.SetActive(outOfSceen);

		if (OnScreenVisuals != null)
			OnScreenVisuals.SetActive(!outOfSceen);


		Vector2 offset = size / 2 + o;

		ssPos.x = Mathf.Clamp(ssPos.x, offset.x, camera.pixelWidth - offset.x);
		ssPos.y = Mathf.Clamp(ssPos.y, offset.y, camera.pixelHeight - offset.y);


		((RectTransform)transform).position = ssPos;

		if (outOfSceen && OutOfSceenVisuals != null) {


			Vector2 norm = (new Vector2(ssPos.x, ssPos.y) - new Vector2(camera.pixelWidth, camera.pixelHeight) / 2).normalized;

			float rotAngle = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg;// + Mathf.PI  ;
			((RectTransform)OutOfSceenVisuals.transform).rotation = Quaternion.Euler(0, 0, rotAngle - 90);

		} else if (OnScreenVisuals != null) {
			ssPos.x = Mathf.Clamp(ssPos.x, size.x / 2, camera.pixelWidth - size.x / 2);
			ssPos.y = Mathf.Clamp(ssPos.y, size.y / 2, camera.pixelHeight - size.y / 2);

			if (DistanceText != null) {
				DistanceText.text = ((TrackingObject.transform.position - DistanceFrom.position).magnitude).ToString("0");
			}
		}
	}
}
