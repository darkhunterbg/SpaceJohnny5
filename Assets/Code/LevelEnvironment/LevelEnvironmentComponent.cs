using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnvironmentComponent : MonoBehaviour
{
	public Material Skybox;

	public void Activate()
	{
		RenderSettings.skybox = Skybox;
		DynamicGI.UpdateEnvironment();
	}
}
