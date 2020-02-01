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
		Random.InitState(System.DateTime.Now.Millisecond);
		SpawnObjects = Random.Range(MinObjects, MaxObjects);

		for (int i = 0; i < SpawnObjects; i++) {
			GameObject spawnable = Instantiate(SpawnableObject, gameObject.transform);

			if (OffsetZone > 0) {
				float delta = (OffsetZone + SphereRadius) - OffsetZone;
				float length = SphereRadius + delta * Random.value;
				Vector3 position = Random.insideUnitSphere.normalized * length;
				position = new Vector3(position.x + Mathf.Sign(position.x) * OffsetZone, position.y + Mathf.Sign(position.y) * OffsetZone, position.z + Mathf.Sign(position.z) * OffsetZone);

				spawnable.transform.localPosition = position;
			}
			else {
				float x = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);
				float y = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);
				float z = RandomSign() * Random.Range(OffsetZone, OffsetZone + SphereRadius);

				spawnable.transform.localPosition = new Vector3(x, y, z);
			}

			spawnable.transform.localScale = new Vector3(Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale), Random.Range(MinScale, MaxScale));
		}
	}

	public int RandomSign()
	{
		return Random.Range(0, 2) * 2 - 1;
	}
}
