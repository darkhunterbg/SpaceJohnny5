using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class GameCursor : MonoBehaviour
{
	private GameLevel _level;
	private Canvas _canvas;

	public void Init(GameLevel level, Canvas canvas)
	{
		_level = level;
		_canvas = canvas;
	}


	public void Update()
	{
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, Input.mousePosition, _canvas.worldCamera, out pos);
		((RectTransform)transform).position = _canvas.transform.TransformPoint(pos);
	}
}

