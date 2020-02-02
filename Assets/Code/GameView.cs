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
	public SSGUIObject PartIndicatorPrefab;
	public SSGUIObject ShipIndicatorPrefab;

	private Canvas _canvas;

	public readonly List<SSGUIObject> TrackingObjects = new List<SSGUIObject>();

	private void Start()
	{
		_canvas = GetComponent<Canvas>();
		_gameLevel = FindObjectOfType<GameLevel>();
		_gameLevel.OnPartDelivered += GameLevel_OnPartDelivered;

		MovementCursor.Init(_gameLevel, _canvas);

		foreach (var part in _gameLevel.Parts) {
			SSGUIObject marker = GameObject.Instantiate(PartIndicatorPrefab, transform);
			marker.Init(_canvas);
			marker.TrackingObject = part.gameObject;
			TrackingObjects.Add(marker);
		}

		SSGUIObject shipMarker = GameObject.Instantiate(ShipIndicatorPrefab, transform);
		shipMarker.Init(_canvas);
		shipMarker.TrackingObject = _gameLevel.Ship.gameObject;
		TrackingObjects.Add(shipMarker);
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

	private void GameLevel_OnPartDelivered(PartLogic part)
	{
		var marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);

		if (marker != null) {
			TrackingObjects.Remove(marker);
			GameObject.Destroy(marker.gameObject);
		}
	}

	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"Time: {timeElapsed:mm\\:ss}";

		bool mouseInput = Input.GetMouseButton(0);

		Cursor.lockState = mouseInput ? CursorLockMode.Confined : CursorLockMode.None;
		Cursor.visible = !mouseInput;

		MovementCursor.gameObject.SetActive(!Cursor.visible);

		foreach (var part in _gameLevel.Parts) {
			//if (part.AttachedToShip) {

			SSGUIObject marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);
			marker.gameObject.SetActive(!part.AttachedToAnything);
			//}
		}

		SSGUIObject shipMarker = TrackingObjects.Find(p => p.TrackingObject == _gameLevel.Ship.gameObject);
		shipMarker.gameObject.SetActive(_gameLevel.Drone.Parts.Any());

		UpdateGravityWellMarkers();
	}

	private void UpdateGravityWellMarkers()
	{
		foreach (var gravityWell in _gameLevel.GravityWells) {
			bool inside = (gravityWell.TestInside(_gameLevel.Drone.transform.position));
			if (inside && !TrackingObjects.Any(t => t.TrackingObject == gravityWell.gameObject)) {
				var obj = GameObject.Instantiate(GravityWellWarningPrefab, transform);
				obj.Init(_canvas);
				obj.TrackingObject = gravityWell.gameObject;
				TrackingObjects.Add(obj);
			}
			if (!inside) {
				var trackingMarker = TrackingObjects.FirstOrDefault(t => t.gameObject == gravityWell.gameObject);
				if (trackingMarker != null) {
					TrackingObjects.Remove(trackingMarker);
					GameObject.Destroy(trackingMarker.gameObject);
				}
			}
		}
	}
}
