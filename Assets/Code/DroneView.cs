using UnityEngine;

public class DroneView : MonoBehaviour
{
	private DroneLogic _droneLogic;
	private GameView _gameView;

	private void Start()
	{
		_gameView = FindObjectOfType<GameView>();
		_droneLogic = GetComponent<DroneLogic>();
		_droneLogic.OnBatteryDrained += OnDroneBatteryDrained;
		_gameView.SetDroneBatteryRemaining(_droneLogic.BatteryRemaining);
	}

	private void OnDroneBatteryDrained(float amount, float battery, bool damage)
	{
		// TODO Show damage text
		_gameView.SetDroneBatteryRemaining(Mathf.Clamp(battery - amount, 0, 100));
	}
}
