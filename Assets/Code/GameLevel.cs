using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
	public Bounds Bounds;
	public LevelEnvironmentsDef LevelEnvironmentsDef;

	public float TimeElapsed => Time.time - _levelStartTime;
	private float _levelStartTime;
	public DroneLogic Drone { get; private set; }
	public ShipLogic Ship { get; private set; }
	private LevelEnvironmentInstance _levelEnvironmentInstnance;
	public List<GravityWell> GravityWells { get; private set; } = new List<GravityWell>();
	public List<PowerUpLogic> PowerUps { get; private set; } = new List<PowerUpLogic>();

	public void Start()
	{
		_levelStartTime = Time.time;
		Drone = FindObjectOfType<DroneLogic>();
		Ship = FindObjectOfType<ShipLogic>();
		_levelEnvironmentInstnance = new LevelEnvironmentInstance(LevelEnvironmentsDef.SelecRandoEnvironment());
		PowerUps.AddRange(FindObjectsOfType<PowerUpLogic>());
	}
}
