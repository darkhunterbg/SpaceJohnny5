using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnvironmentInstance : MonoBehaviour
{
	private GameObject _instance;

	public LevelEnvironmentInstance(GameObject env)
	{
		_instance = GameObject.Instantiate(env);
		_instance.GetComponent<LevelEnvironmentComponent>().Activate();
	}
}
