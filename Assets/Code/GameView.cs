using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class GameView : MonoBehaviour
{
	public Text DroneBatteryText;
	public Slider DroneBatteryProgress;
	public Text TimeElapsedText;
	public Text PartsLeftText;

	private GameLevel _gameLevel;

	public GameCursor MovementCursor;

	public SSGUIObject GravityWellWarningPrefab;
	public SSGUIObject PartIndicatorPrefab;
	public SSGUIObject ShipIndicatorPrefab;
	public SSGUIObject BatteryIndicatorPrefab;

	public float BatteryIndicatorAppearanceRange = 30;

	public bool AlwayShowShipIndicator = true;

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


		foreach (var part in _gameLevel.PowerUps) {
			SSGUIObject marker = GameObject.Instantiate(BatteryIndicatorPrefab, transform);
			marker.gameObject.SetActive(false);
			marker.Init(_canvas, _gameLevel.Drone.transform);
			marker.TrackingObject = part.gameObject;
			TrackingObjects.Add(marker);
		}

		SSGUIObject shipMarker = GameObject.Instantiate(ShipIndicatorPrefab, transform);
		shipMarker.Init(_canvas, _gameLevel.Drone.transform);
		shipMarker.TrackingObject = _gameLevel.Ship.gameObject;
		TrackingObjects.Add(shipMarker);
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
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
		float battery = Mathf.Ceil(batteryRemaining);
		DroneBatteryText.text = $"{(int)battery}";
		DroneBatteryProgress.value = Mathf.CeilToInt(battery / 5);
	}

	public void PartDelivered(PartLogic part)
	{
		var marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);

		if (marker != null) {
			TrackingObjects.Remove(marker);
			Destroy(marker.gameObject);
		}
	}

	public void PowerUpPicked(PowerUpLogic powerUp)
	{
		if (powerUp == null)
			return;

		var marker = TrackingObjects.Find(p => p.TrackingObject == powerUp.gameObject);

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
		int partsRemaining = partsTotal - partsDelivered;

		//if (partsCarried == 0) {
		//	PartsLeftText.text = $"{partsDelivered}/{partsTotal}";
		//} else {
		PartsLeftText.text = $"{partsCarried}/{partsRemaining}";
		//}
	}

	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"{timeElapsed:mm\\:ss\\:ff}";

		bool mouseInput = Input.GetMouseButton(0);

		Cursor.lockState = mouseInput ? CursorLockMode.Confined : CursorLockMode.None;
		Cursor.visible = !mouseInput;

		if (!MovementCursor.gameObject.activeSelf && !Cursor.visible) {
			MovementCursor.Prepare();
		}

		MovementCursor.gameObject.SetActive(!Cursor.visible);

		UpdatePartsIndicators();
		UpdatePowerIndicators();
		UpdateGravityWellMarkers();
		UpdatePartsInfo();
		UpdateShipIndicator();
	}

	private void UpdateShipIndicator()
	{
		SSGUIObject shipMarker = TrackingObjects.Find(p => p.TrackingObject == _gameLevel.Ship.gameObject);

		shipMarker.gameObject.SetActive(AlwayShowShipIndicator || _gameLevel.Drone.Parts.Any());

	}

	private void UpdatePartsIndicators()
	{
		foreach (var part in _gameLevel.Ship.Parts) {
			SSGUIObject marker = TrackingObjects.Find(p => p.TrackingObject == part.gameObject);
			marker.gameObject.SetActive(!part.AttachedToAnything);
		}
	}

	private void UpdatePowerIndicators()
	{
		foreach (var powerUp in _gameLevel.PowerUps) {
			bool inRange = (powerUp.transform.position - _gameLevel.Drone.transform.position).magnitude < BatteryIndicatorAppearanceRange;
			SSGUIObject marker = TrackingObjects.Find(p => p.TrackingObject == powerUp.gameObject);

			marker.gameObject.SetActive(inRange);
		}


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
