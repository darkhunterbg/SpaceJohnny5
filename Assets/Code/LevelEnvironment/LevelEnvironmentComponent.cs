using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class LevelEnvironmentComponent : MonoBehaviour
{
	public Material Skybox;
	public Color ambientColor;

#if UNITY_EDITOR
	private void OnEnable()
	{
		if(!Application.isPlaying)
			Activate();
	}
#endif

	public void Activate()
	{
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = ambientColor;
		RenderSettings.skybox = Skybox;

		RemoveOtherDirectionalLights();
	}

	private void RemoveOtherDirectionalLights()
	{
		var dirL = GameObject.Find("Directional Light");

		if (dirL != null && dirL.transform.root != transform) {
			if (!Application.isPlaying) {
				DestroyImmediate(dirL);
			} else { 
				Destroy(dirL);
			}
		}
	}
}
