using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
	public delegate void OnBatteryChangedCallback(float amount, float battery, bool damage);
	public event OnBatteryChangedCallback OnBatteryChanged;

	public float ThrustersBatteryDrain = 1.0f;
	public AudioSource HurtAudio;
	public AudioSource BatteryPickUpAudio;
	
	public bool Dead => _batteryRemaining <= 0;
	public float BatteryRemaining => _batteryRemaining;
	private float _batteryRemaining = 100; // percentage 0..100
	
	private DroneController _droneController;
	private readonly List<PartLogic> _parts = new List<PartLogic>();
	
	private void Start()
	{
		_droneController = GetComponent<DroneController>();
	}

	private void Damage(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: true);
	}

	private void ChargeBattery(float amount, GameObject batteryObjects) // percentage 0..100
	{
		if (Dead) {
			return;
		}
		
		OnBatteryChanged?.Invoke(amount, _batteryRemaining, false);
		_batteryRemaining += amount;
		
		if (_batteryRemaining > 100) {
			_batteryRemaining = 100;
		}

		Destroy(batteryObjects);
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if (!other.collider.enabled) {
			return;
		}

		string hitLayer = LayerMask.LayerToName(other.collider.gameObject.layer);
		Debug.Log($"Collision: {LayerMask.LayerToName(other.collider.gameObject.layer)}");

		switch (hitLayer) {
			case "Obstacles":
				Damage(20);
				HurtAudio.Play();
				break;
			
			case "Parts":
				PickupPart(other.gameObject);
				break;
			
			case "Ship":
				DeliverParts(other.gameObject);
				break;
			
			case "PowerUps":
				ChargeBattery(20, other.gameObject);
				BatteryPickUpAudio.Play();
				break;
		}
	}

	private void DeliverParts(GameObject ship)
	{
		if (_parts.Count > 0) {
			_parts[0].Attach(ship.transform);
			_parts.Clear();
		}
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
			//in case that your battery dries out
			EndLevel(damage);

			return;
		}
		
		OnBatteryChanged?.Invoke(-amount, _batteryRemaining, damage);
		_batteryRemaining -= amount;
		
		if (_batteryRemaining <= 0) {
			_batteryRemaining = 0;
			EndLevel(damage);
		}
	}

	private void EndLevel(bool damage)
	{
		var ship = FindObjectOfType<ShipLogic>();
		var gameLevel = FindObjectOfType<GameLevel>();

		var gameResult = new GameResult
		{
			Type = damage ? GameResultType.Destroyed : GameResultType.BatteryDepleted,
			Time = gameLevel.TimeElapsed,
			PartsReceived = ship.PartsReceived,
			PartsTotal = ship.PartsTotal,
		};

		Die(gameResult);
	}

	private void Die(GameResult result)
	{
		// TODO Explode drone if needed
		// TODO Wait couple of seconds
		Game.Instance.StartLevel("Defeat", result);
	}

	private void Update()
	{
		if (_droneController.ThrustersActive) {
			DrainBattery(ThrustersBatteryDrain * Time.deltaTime, damage: false);
		}
	}
}
