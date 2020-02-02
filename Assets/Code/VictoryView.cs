using System;
using UnityEngine;
using UnityEngine.UI;

public class VictoryView : MonoBehaviour
{
	public Text TimeText;
	public Text PartsText;
	
	public void Start()
	{
		var gameResult = (GameResult) Game.Instance.LevelParams;
		if (gameResult != null)
		{
			TimeSpan timeElapsed = TimeSpan.FromSeconds(gameResult.Time);
			TimeText.text = $"YOUR TIME: {timeElapsed:mm\\:ss\\:ff}";
			PartsText.text = $"PARTS DELIVERED: {gameResult.PartsDelivered}/{gameResult.PartsTotal}";
		}
	}

	public void OnPlayAgainPressed()
	{
		Game.Instance.StartLevel("Game");
	}
	
	public void OnQuitPressed()
	{
		Game.Instance.QuitGame();
	}
}
