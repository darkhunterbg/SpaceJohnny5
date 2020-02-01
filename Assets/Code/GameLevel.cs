using UnityEngine;

public class GameLevel : MonoBehaviour
{
	public Bounds Bounds;
	
	public float TimeElapsed => Time.time - _levelStartTime;
	
	private float _levelStartTime;

	public void Start()
	{
		_levelStartTime = Time.time;
	}
}
