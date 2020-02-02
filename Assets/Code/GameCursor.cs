using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameCursor : MonoBehaviour
{
	private GameLevel _level;
	private Canvas _canvas;


	public GameObject Indicator;
	public RectTransform Direction;

	private List<GameObject> _arrows = new List<GameObject>();

	public float ArrowDistance;

	private void Start()
	{
		for (int i = 0; i < Direction.childCount; ++i) {
			_arrows.Add(Direction.GetChild(i).gameObject);
		}
		Prepare();
	}

	public void Init(GameLevel level, Canvas canvas)
	{
		_level = level;
		_canvas = canvas;
	}


	public void Prepare()
	{

		foreach (var c in _arrows)
			c.gameObject.SetActive(false);
	}

	public void Update()
	{
		Camera camera = Camera.main;
		Vector2 camSize = new Vector2(camera.pixelWidth, camera.pixelHeight);

		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, Input.mousePosition, _canvas.worldCamera, out pos);
		((RectTransform)Indicator.transform).position = _canvas.transform.TransformPoint(pos);
		Vector2 curosDir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - camSize / 2.0f;
		Vector2 norm = curosDir.normalized;


		float rotAngle = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg; ;
		((RectTransform)Direction.transform).rotation = Quaternion.Euler(0, 0, rotAngle);

		foreach (var c in _arrows)
			c.gameObject.SetActive(false);

		for (int i = 0; i < (int)(curosDir.magnitude / ArrowDistance); ++i) {

			if (i >= _arrows.Count)
				break;

			var c = _arrows[i];

			RectTransform rt = ((RectTransform)c.transform);

			float distance = (i + 1) * ArrowDistance;// - (rt.rect.width) / 2.0f;

			c.gameObject.SetActive(true);
			rt.position = new Vector3(norm.x * distance, norm.y * distance, c.transform.position.z) + new Vector3(camSize.x / 2.0f, camSize.y / 2.0f, 0);
		}

	}
}

