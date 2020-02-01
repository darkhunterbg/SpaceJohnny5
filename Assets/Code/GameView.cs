using System;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
	public Text DroneBatteryText;
	public Text TimeElapsedText;

	private GameLevel _gameLevel;

	private void Start()
	{
		_gameLevel = FindObjectOfType<GameLevel>();
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

	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"Time: {timeElapsed:mm\\:ss}";
	}
}
