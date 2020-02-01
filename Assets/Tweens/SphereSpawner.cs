using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
	public float MinObjects;
	public float MaxObjects;
	public float SphereRadius;
	public float MinScale = 1;
	public float MaxScale = 3;


	public GameObject SpawnableObject;

	[Header("Preview")]
	public float SpawnObjects;

    // Start is called before the first frame update
    void Start()
    {
		SpawnObjects = Random.Range(MinObjects, MaxObjects);

		for (int i = 0; i < SpawnObjects; i++) {
			GameObject spawnable = Instantiate(SpawnableObject, gameObject.transform);
			spawnable.transform.localPosition = new Vector3(Random.Range(-SphereRadius, SphereRadius), Random.Range(-SphereRadius, SphereRadius), Random.Range(-SphereRadius, SphereRadius));
			spawnable.transform.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));
		}
	}
}
