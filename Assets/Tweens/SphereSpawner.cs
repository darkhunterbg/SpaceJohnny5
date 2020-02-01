using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
	public int MinObjects;
	public int MaxObjects;
	public int SphereRadius;
	public float MinScale = 1;
	public float MaxScale = 3;
	public int OffsetZone = 0;

	public GameObject SpawnableObject;

	[Header("Preview")]
	public float SpawnObjects;

    // Start is called before the first frame update
    void Start()
    {
		SpawnObjects = Random.Range(MinObjects, MaxObjects);

		for (int i = 0; i < SpawnObjects; i++) {
			GameObject spawnable = Instantiate(SpawnableObject, gameObject.transform);
			float x = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);
			float y = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);
			float z = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);

			spawnable.transform.localPosition = new Vector3(x, y, z);
			spawnable.transform.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));
		}
	}

	public int RandomSign()
	{
		return Random.Range(0, 2) * 2 - 1;
	}
}
