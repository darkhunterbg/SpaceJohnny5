using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
	public delegate void OnBatteryChangedCallback(float amount, float battery, bool damage);
	public event OnBatteryChangedCallback OnBatteryChanged;
	public bool Dead => _batteryRemaining <= 0;

	public float BatteryRemaining => _batteryRemaining;
	private float _batteryRemaining = 100; // percentage 0..100
	private GameLevel _gameLevel;
	private List<PartLogic> _parts = new List<PartLogic>();
	
	private void Start()
	{
		_gameLevel = FindObjectOfType<GameLevel>();
	}

	public void DrainBattery(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: false);
	}

	public void Damage(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: true);
	}

	public void ChargeBattery(float amount) // percentage 0..100
	{
		if (Dead) {
			return;
		}
		
		OnBatteryChanged?.Invoke(amount, _batteryRemaining, false);
		_batteryRemaining += amount;
		
		if (_batteryRemaining > 100) {
			_batteryRemaining = 100;
		}
	}
	
	public void OnCollisionEnter(Collision other)
	{
		if (!other.collider.enabled) {
			return;
		}

		string hitLayer = LayerMask.LayerToName(other.collider.gameObject.layer);
		Debug.Log($"Collision: {LayerMask.LayerToName(other.collider.gameObject.layer)}");

		switch (hitLayer) {
			case "Obstacles":
				Damage(20);
				break;
			
			case "Parts":
				PickupPart(other.gameObject);
				break;
			
			case "Ship":
				DropParts(other.gameObject);
				break;
			
			case "Battery":
				ChargeBattery(20);
				break;
		}
	}

	private void DropParts(GameObject ship)
	{
	}

	private void PickupPart(GameObject part)
	{
		var partLogic = part.GetComponent<PartLogic>();

		if (_parts.Contains(partLogic)) {
			// Already attached
			return;
		}
		
		if (_parts.Count == 0) {
			partLogic.Attach(transform);	
		} else {
			partLogic.Attach(_parts[_parts.Count - 1].transform);
		}
		
		_parts.Add(partLogic);
	}

	private void DrainBattery(float amount, bool damage)
	{
		if (Dead) {
			return;
		}
		
		OnBatteryChanged?.Invoke(-amount, _batteryRemaining, damage);
		_batteryRemaining -= amount;
		
		if (_batteryRemaining <= 0) {
			_batteryRemaining = 0;
			
			var gameResult = new GameResult {
				Type = damage ? GameResultType.Destroyed : GameResultType.BatteryDepleted,
				Time = _gameLevel.TimeElapsed,
			};
			
			Die(gameResult);
		}
	}
	
	private void Die(GameResult result)
	{
		// TODO Explode drone if needed
		// TODO Wait couple of seconds
		Game.Instance.StartLevel("Defeat", result);
	}
}
