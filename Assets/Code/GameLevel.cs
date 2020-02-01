using UnityEngine;

public class GameLevel : MonoBehaviour
{
	public Bounds Bounds;
	
	public float TimeElapsed => Time.time - _levelStartTime;
	
	private float _levelStartTime;

	public DroneController Drone { get; private set; }

	public void Start()
	{
		_levelStartTime = Time.time;
		Drone = FindObjectOfType<DroneController>();
	}
}
