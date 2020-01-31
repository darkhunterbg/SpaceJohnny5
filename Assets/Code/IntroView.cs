using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroView : MonoBehaviour
{
	public void OnPlayButtonPressed()
	{
		Game.Instance.StartLevel("Game");
	}
}
