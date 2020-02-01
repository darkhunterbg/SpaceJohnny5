using UnityEngine;

public class GameLevel : MonoBehaviour
{
	public Bounds Bounds;
	public LevelEnvironmentsDef LevelEnvironmentsDef;

	public float TimeElapsed => Time.time - _levelStartTime;
	private float _levelStartTime;
	public DroneController Drone { get; private set; }

	private LevelEnvironmentInstance _levelEnvironmentInstnance;

	public void Start()
	{
		_levelStartTime = Time.time;
		Drone = FindObjectOfType<DroneController>();
		_levelEnvironmentInstnance = new LevelEnvironmentInstance(LevelEnvironmentsDef.SelecRandoEnvironment());
	}
}
