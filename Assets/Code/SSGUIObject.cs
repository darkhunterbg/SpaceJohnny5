using UnityEngine;

public class SSGUIObject : MonoBehaviour
{
	public GameObject TrackingObject;

	private Canvas _canvas;

	public GameObject Visuals;

	public GameObject RotateVisual;
	public GameObject OrientatedVisual;

	public void Init(Canvas canvas)
	{
		this._canvas = canvas;
	}

	public void Update()
	{
		if (TrackingObject == null)
			return;

		var camera = Camera.main; 
		Vector2 ssPos = RectTransformUtility.WorldToScreenPoint(camera, TrackingObject.transform.position);

		Vector2 size = ((RectTransform)transform).rect.size;

		bool outOfSceen = ssPos.x < 0 || ssPos.x > camera.pixelWidth || ssPos.y < 0 || ssPos.y > camera.pixelHeight;

		Visuals.SetActive(outOfSceen);

		if (outOfSceen) {

			ssPos.x = Mathf.Clamp(ssPos.x, size.x / 2, camera.pixelWidth - size.x / 2);
			ssPos.y = Mathf.Clamp(ssPos.y, size.y / 2, camera.pixelHeight - size.y / 2);

			//RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, ssPos, camera, out pos);

			((RectTransform)transform).position = ssPos;

			
		}
	}
}
