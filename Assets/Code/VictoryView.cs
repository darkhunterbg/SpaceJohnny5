using System;
using UnityEngine;
using UnityEngine.UI;

public class VictoryView : MonoBehaviour
{
	public Text TimeText;
	public GameObject Ship;
	public float ShipSpeed = 25;
	
	public void Start()
	{
		var gameResult = (GameResult) Game.Instance.LevelParams;
		if (gameResult != null)
		{
			TimeSpan timeElapsed = TimeSpan.FromSeconds(gameResult.Time);
			TimeText.text = $"YOUR TIME: {timeElapsed:mm\\:ss\\:ff}";
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
	
	public void Update()
	{
		Ship.transform.position += Ship.transform.forward * ShipSpeed * Time.deltaTime;
	}
}
