using UnityEngine;

public class GameLevel : MonoBehaviour
{
	public class LevelEnvironmentInstance
	{
		private GameObject _currentEnvironmentPrefab;
		
		public LevelEnvironmentInstance(GameObject environmentPrefab)
		{
			_currentEnvironmentPrefab = GameObject.Instantiate(environmentPrefab);
			_currentEnvironmentPrefab.GetComponent<LevelEnvironmentComponent>().Activate();
		}
	}


	public Bounds Bounds;
	public LevelEnvironmentsDef LevelEnvironmentsDef;
	
	public float TimeElapsed => Time.time - _levelStartTime;
	private float _levelStartTime;
	private LevelEnvironmentInstance _levelEnvironment;
	
	
	public void Start()
	{
		_levelStartTime = Time.time;
		_levelEnvironment = new LevelEnvironmentInstance(LevelEnvironmentsDef.SelecRandoEnvironment());
	}
}
