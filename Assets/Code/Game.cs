using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Game : MonoBehaviour
{
	private const string LevelPrefix = "Level_";
	private const float TransitionTime = 0.5f;

	public static Game Instance;
	public object LevelParams;
	
	private Dictionary<string, int> _levels = new Dictionary<string, int>();
	private CanvasGroup _curtainCanvas;
	private bool CurtainUp => _curtainCanvas.alpha == 0;

	public void Awake()
	{
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		int i = 0;

		while (true) {
			string path = SceneUtility.GetScenePathByBuildIndex(i);

			if (string.IsNullOrEmpty(path)) {
				break;
			}

			string sceneName = Path.GetFileNameWithoutExtension(path);
			int ind = sceneName.IndexOf(LevelPrefix, StringComparison.Ordinal);

			if (ind >= 0) {
				_levels[sceneName.Substring(ind + LevelPrefix.Length)] = i;
			}

			++i;
		}

		_curtainCanvas = transform.Find("CurtainCanvas").GetComponentInChildren<CanvasGroup>();
		StartLevel("Intro");
	}

	public void StartLevel(string level, object levelParams = null)
	{
		StartCoroutine(LoadLevelCrt(level, levelParams));
	}

	public void QuitGame()
	{
		StartCoroutine(QuitGameCrt());
	}

	private IEnumerator<YieldInstruction> QuitGameCrt()
	{
		if (CurtainUp) {
			yield return StartCoroutine(CurtainDownCrt());
		}

#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private IEnumerator<YieldInstruction> LoadLevelCrt(string level, object levelParams)
	{
		if (CurtainUp) {
			yield return StartCoroutine(CurtainDownCrt());
		}

		LevelParams = levelParams;
		yield return SceneManager.LoadSceneAsync(_levels[level]);
		yield return StartCoroutine(CurtainUpCrt());
	}

	private IEnumerator<YieldInstruction> CurtainUpCrt()
	{
		float startTime = Time.time;
		float endTime = startTime + TransitionTime;

		while (true) {
			float now = Time.time;

			if (now >= endTime) {
				break;
			}

			_curtainCanvas.alpha = 1 - (now - startTime) / TransitionTime;
			yield return new WaitForSeconds(0);
		}

		_curtainCanvas.blocksRaycasts = false;
		_curtainCanvas.alpha = 0;
	}

	private IEnumerator<YieldInstruction> CurtainDownCrt()
	{
		_curtainCanvas.blocksRaycasts = true;
		float startTime = Time.time;
		float endTime = startTime + TransitionTime;

		while (true) {
			float now = Time.time;

			if (now >= endTime) {
				break;
			}

			_curtainCanvas.alpha = (now - startTime) / TransitionTime;
			yield return new WaitForSeconds(0);
		}

		_curtainCanvas.alpha = 1;
	}
}
