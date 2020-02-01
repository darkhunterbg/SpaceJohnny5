using UnityEngine;

public class DroneLogic : MonoBehaviour
{
	public delegate void OnBatteryDrainedCallback(float amount, float battery, bool damage);
	public event OnBatteryDrainedCallback OnBatteryDrained;
	public bool Dead => _batteryRemaining <= 0;

	public float BatteryRemaining => _batteryRemaining;
	private float _batteryRemaining; // percentage 0..100
	private GameLevel _gameLevel;
	
	private void Start()
	{
		_batteryRemaining = 100;
		_gameLevel = FindObjectOfType<GameLevel>();
	}

	public void DrainBattery(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: false);
	}

	private void Damage(float amount) // percentage 0..100
	{
		DrainBattery(amount, damage: true);
	}
	
	private void DrainBattery(float amount, bool damage)
	{
		if (Dead) {
			return;
		}
		
		OnBatteryDrained?.Invoke(amount, _batteryRemaining, damage);
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
