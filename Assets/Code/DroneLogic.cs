using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
	public delegate void OnBatteryChangedCallback(float amount, float battery, bool damage);
	public event OnBatteryChangedCallback OnBatteryChanged;

	public float ThrustersBatteryDrain = 1.0f;
	public AudioSource HurtAudio;
	public AudioSource BatteryPickUpAudio;
	public AudioSource DeliverPartAudio;
	
	public bool Dead => _batteryRemaining <= 0;
	public float BatteryRemaining => _batteryRemaining;
	private float _batteryRemaining = 100; // percentage 0..100
	
	private DroneController _droneController;
	private readonly List<PartLogic> _parts = new List<PartLogic>();
	public IList<PartLogic> Parts => _parts;

	private GameView _view;
	private GameLevel _level;
	[SerializeField] private Animator _animator;
	[SerializeField] private ParticleSystem _shockParticles;


#pragma warning disable 0618
	private void Start()
	{
		_droneController = GetComponent<DroneController>();
		_view = FindObjectOfType<GameView>();
		_level = FindObjectOfType<GameLevel>();

		if(_animator == null) {
			_animator.GetComponentInChildren<Animator>();
		}

		_shockParticles.enableEmission = false;
	}

	private void Damage(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: true);
	}

	private void ChargeBattery(float amount) // percentage 0..100
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
	
	private void OnCollisionEnter(Collision other)
	{
		if (!other.collider.enabled) {
			return;
		}

		string hitLayer = LayerMask.LayerToName(other.collider.gameObject.layer);
		Debug.Log($"Collision: {LayerMask.LayerToName(other.collider.gameObject.layer)}");

		switch (hitLayer) {
			case "Obstacles":
				HitObstacle(other);
				break;
			
			case "Parts":
				PickupPart(other.gameObject);
				break;
			
			case "Ship":
				DeliverParts(other.gameObject);
				break;
			
			case "PowerUps":
				PickupPowerUp(other.gameObject);
				break;
		}
	}

	private void HitObstacle(Collision other)
	{
		_view.ShowWarningMessage("-20 BATTERY");
		Vector3 hitCenter = other.collider.bounds.center;
		Vector3 normal = (transform.position - hitCenter).normalized;
		_droneController.ApplyHit(normal * 20);
		Damage(20);
		HurtAudio.Play();
		_shockParticles.enableEmission = true;
		StartCoroutine(StopShockParticlesAfterTime(1.5f));
	}

	private IEnumerator StopShockParticlesAfterTime (float time)
	{
		yield return new WaitForSeconds(time);
		_shockParticles.enableEmission = false;
	}
#pragma warning restore 0618

	private void PickupPowerUp(GameObject powerUp)
	{
		var powerUpLogic = powerUp.GetComponent<PowerUpLogic>();

		switch (powerUpLogic.Type) {
			case PowerUpType.Battery:
				_view.ShowWarningMessage("+20 BATTERY");
				ChargeBattery(20);
				break;
			case PowerUpType.Speed:
				_view.ShowWarningMessage("SPEED UP!");
				_droneController.IncreaseVelocityFor(time: 4);
				break;
		}
		
		_view.PowerUpPicked(powerUpLogic);
		_level.PowerUps.Remove(powerUpLogic);
		Destroy(powerUp);
		BatteryPickUpAudio.Play();
	}

	private void DeliverParts(GameObject ship)
	{
		if (_parts.Count > 0) {
			_view.ShowWarningMessage($"{_parts.Count} PART(S) DELIVERED");
			_parts[0].Attach(ship.transform);
			_parts.Clear();
			DeliverPartAudio.Play();
		}
	}

	private void PickupPart(GameObject part)
	{
		var partLogic = part.GetComponent<PartLogic>();

		if (partLogic.AttachedToAnything) {
			// Already attached
			return;
		}
		
		_view.ShowWarningMessage("PART PICKED UP");
		
		if (_parts.Count == 0) {
			partLogic.Attach(transform);	
		} else {
			partLogic.Attach(_parts[_parts.Count - 1].transform);
		}
		
		_parts.Add(partLogic);

		// play animation
		_animator.SetTrigger("Pickup");
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
			_view.ShowWarningMessage(damage? "DRONE DESTROYED" : "BATTERY DEPLETED", persistent: true);
			StartCoroutine(EndLevelCrt(damage));
		}
	}

	private IEnumerator<YieldInstruction> EndLevelCrt(bool damage)
	{
		_level.StopTimer();
		_droneController.State = DroneControllerState.Dead;
		var ship = FindObjectOfType<ShipLogic>();
		var gameLevel = FindObjectOfType<GameLevel>();

		var gameResult = new GameResult
		{
			Type = damage ? GameResultType.Destroyed : GameResultType.BatteryDepleted,
			Time = gameLevel.TimeElapsed,
			PartsDelivered = ship.PartsDelivered,
			PartsTotal = ship.PartsTotal,
		};
		
		foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>()) {
			renderer.enabled = false;
		}

		foreach (var light in gameObject.GetComponentsInChildren<Light>()) {
			light.enabled = false;
		}
		
		// TODO Explode drone if needed
		
		yield return new WaitForSeconds(5);
		Game.Instance.StartLevel("Defeat", gameResult);
	}
	
	private void Update()
	{
		if (_droneController.ThrustersActive) {
			DrainBattery(ThrustersBatteryDrain * Time.deltaTime, damage: false);
		}
	}
}
