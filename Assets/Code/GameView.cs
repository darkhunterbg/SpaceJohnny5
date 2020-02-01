using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
	public void OnTestVictoryButtonPressed()
	{
		Game.Instance.StartLevel("Victory");
	}

	public void OnTestDefeatButtonPressed()
	{
		Game.Instance.StartLevel("Defeat");
	}
}
