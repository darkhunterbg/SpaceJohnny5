﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryView : MonoBehaviour
{
	public void OnPlayAgainPressed()
	{
		Game.Instance.StartLevel("Game");
	}
	
	public void OnQuitPressed()
	{
		Game.Instance.QuitGame();
	}
}