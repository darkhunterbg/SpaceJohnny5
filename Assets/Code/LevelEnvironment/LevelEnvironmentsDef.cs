using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEnvironmentsDef", menuName = "Defs/LevelEnvironmentDef")]
public class LevelEnvironmentsDef : ScriptableObject
{
	public GameObject[] EnvironmentVariants;

	public GameObject SelecRandoEnvironment()
	{
		return EnvironmentVariants[Random.Range(0, EnvironmentVariants.Length)];
	}
}
