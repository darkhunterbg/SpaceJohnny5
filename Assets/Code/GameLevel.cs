using System.Collections.Generic;
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

	public event System.Action<PartLogic> OnPartDelivered;

	public List<GravityWell> GravityWells { get; private set; } = new List<GravityWell>();
	public List<PartLogic> Parts { get; private set; } = new List<PartLogic>();

	public void Start()
	{
		_levelStartTime = Time.time;
		Drone = FindObjectOfType<DroneLogic>();
		Ship = FindObjectOfType<ShipLogic>();
		_levelEnvironmentInstnance = new LevelEnvironmentInstance(LevelEnvironmentsDef.SelecRandoEnvironment());

		Parts.AddRange(FindObjectsOfType<PartLogic>());
	}

	public void BroadcastPartDelivered(PartLogic part)
	{
		Parts.Remove(part);
		OnPartDelivered?.Invoke(part);
	}
}
