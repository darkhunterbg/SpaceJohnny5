using System;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
	public Text DroneBatteryText;
	public Text TimeElapsedText;
	public Text PartsLeftText;
	
	private GameLevel _gameLevel;

	public GameCursor MovementCursor;

	private Canvas _canvas;

	private void Start()
	{
		_canvas = GetComponent<Canvas>();
		_gameLevel = FindObjectOfType<GameLevel>();

		MovementCursor.Init(_gameLevel, _canvas);
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
	
	private void Update()
	{
		TimeSpan timeElapsed = TimeSpan.FromSeconds(_gameLevel.TimeElapsed);
		TimeElapsedText.text = $"Time: {timeElapsed:mm\\:ss}";

		bool mouseInput = Input.GetMouseButton(0);

		Cursor.lockState = mouseInput ? CursorLockMode.Confined : CursorLockMode.None;
		Cursor.visible = !mouseInput;

		MovementCursor.gameObject.SetActive(!Cursor.visible);
	}
}
