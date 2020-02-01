using UnityEngine;

public class DroneView : MonoBehaviour
{
	private DroneLogic _droneLogic;
	private GameView _gameView;

	private void Start()
	{
		_gameView = FindObjectOfType<GameView>();
		_droneLogic = GetComponent<DroneLogic>();
		_droneLogic.OnBatteryChanged += OnDroneBatteryChanged;
		_gameView.SetDroneBatteryRemaining(_droneLogic.BatteryRemaining);
	}

	private void OnDroneBatteryChanged(float amount, float battery, bool damage)
	{
		// TODO Show damage/charge text
		_gameView.SetDroneBatteryRemaining(Mathf.Clamp(battery + amount, 0, 100));
	}
}
