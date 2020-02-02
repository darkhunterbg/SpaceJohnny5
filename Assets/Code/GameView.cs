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

		MovementCursor.Init(_gameLevel, _canvas);

		foreach (var part in _gameLevel.Ship.Parts) {
			SSGUIObject marker = GameObject.Instantiate(PartIndicatorPrefab, transform);
			marker.Init(_canvas, _gameLevel.Drone.transform);
			marker.TrackingObject = part.gameObject;
			TrackingObjects.Add(marker);
		}

		SSGUIObject shipMarker = GameObject.Instantiate(ShipIndicatorPrefab, transform);
		shipMarker.Init(_canvas, _gameLevel.Drone.transform);
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
	
	public void PartDelivered(PartLogic part)
	{
		var marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);

		if (marker != null) {
			TrackingObjects.Remove(marker);
			Destroy(marker.gameObject);
		}
	}
	
	public void UpdatePartsInfo()
	{
		int partsCarried = _gameLevel.Drone.Parts.Count;
		int partsTotal = _gameLevel.Ship.PartsTotal;
		int partsDelivered = _gameLevel.Ship.PartsDelivered;
		
		if (partsCarried == 0)
		{
			PartsLeftText.text = $"Parts: {partsDelivered}/{partsTotal}";
		}
		else
		{
			PartsLeftText.text = $"Parts: {partsCarried} {partsDelivered}/{partsTotal}";
		}
	}

	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"Time: {timeElapsed:mm\\:ss\\:ff}";

		bool mouseInput = Input.GetMouseButton(0);

		Cursor.lockState = mouseInput ? CursorLockMode.Confined : CursorLockMode.None;
		Cursor.visible = !mouseInput;

		MovementCursor.gameObject.SetActive(!Cursor.visible);

		foreach (var part in _gameLevel.Ship.Parts) {
			//if (part.AttachedToShip) {

			SSGUIObject marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);
			marker.gameObject.SetActive(!part.AttachedToAnything);
			//}
		}

		SSGUIObject shipMarker = TrackingObjects.Find(p => p.TrackingObject == _gameLevel.Ship.gameObject);
		shipMarker.gameObject.SetActive(_gameLevel.Drone.Parts.Any());

		UpdateGravityWellMarkers();
		UpdatePartsInfo();
	}

	private void UpdateGravityWellMarkers()
	{
		foreach (var gravityWell in _gameLevel.GravityWells) {
			bool inside = (gravityWell.TestInside(_gameLevel.Drone.transform.position));
			if (inside && !TrackingObjects.Any(t => t.TrackingObject == gravityWell.gameObject)) {
				var obj = GameObject.Instantiate(GravityWellWarningPrefab, transform);
				obj.Init(_canvas, _gameLevel.Drone.transform);
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
