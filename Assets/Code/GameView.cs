using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class GameView : MonoBehaviour
{
	public Text DroneBatteryText;
	public Text TimeElapsedText;
	public Text PartsLeftText;
	
	private GameLevel _gameLevel;

	public GameCursor MovementCursor;

	public SSGUIObject GravityWellWarningPrefab;

	private Canvas _canvas;

	public readonly List<SSGUIObject> TrackingObjects = new List<SSGUIObject>();

	private void Start()
	{
		_canvas = GetComponent<Canvas>();
		_gameLevel = FindObjectOfType<GameLevel>();

		MovementCursor.Init(_gameLevel, _canvas);
	}

	public void OnTestVictoryButtonPressed()
	{
		Game.Instance.StartLevel("Victory");
	}

	public void OnTestDefeatButtonPressed()
	{
		Game.Instance.StartLevel("Defeat");
	}

	public void SetDroneBatteryRemaining(float batteryRemaining)
	{
		// SGG show bar
		DroneBatteryText.text = $"Battery: {batteryRemaining:000.00}";
	}

	public void SetPartsLeft(int parts, int total)
	{
		PartsLeftText.text = $"Pars: {parts}/{total}";
	}
	
	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"Time: {timeElapsed:mm\\:ss}";

		bool mouseInput = Input.GetMouseButton(0);

		Cursor.lockState = mouseInput ? CursorLockMode.Confined : CursorLockMode.None;
		Cursor.visible = !mouseInput;

		MovementCursor.gameObject.SetActive(!Cursor.visible);

		foreach (var gravityWell in _gameLevel.GravityWells) {
			bool inside = (gravityWell.TestInside(_gameLevel.Drone.transform.position));
			if (inside && !TrackingObjects.Any(t => t.TrackingObject == gravityWell.gameObject)) {
				var obj = GameObject.Instantiate(GravityWellWarningPrefab, transform);
				obj.Init(_canvas);
				obj.TrackingObject = gravityWell.gameObject;
				TrackingObjects.Add(obj);
			}
			if(!inside) {
				var trackingMarker = TrackingObjects.FirstOrDefault(t => t.gameObject == gravityWell.gameObject);
				if (trackingMarker != null) {
					TrackingObjects.Remove(trackingMarker);
					GameObject.Destroy(trackingMarker.gameObject);
				}
			}
		}
	}
}
