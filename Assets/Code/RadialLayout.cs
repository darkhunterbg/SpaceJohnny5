using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Extensions/Radial Layout")]
public class RadialLayout : MonoBehaviour
{
	public float fDistance;
	[Range(0f, 360f)]
	public float Angle, StartAngle;

	public PingPongValue Contraction;

	void Awake()
	{
		CalculateRadial(fDistance);
	}

	public void CalculateRadial(float distance)
	{
		if (transform.childCount == 0)
			return;

		int childrenToFormat = 0;
		childrenToFormat = transform.childCount;

		//float fOffsetAngle = Angle / childrenToFormat;

		float fAngle = StartAngle;
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
			child.localPosition = vPos * distance;

			fAngle += Angle;
		}
	}

	private void Update()
	{
		CalculateRadial(Contraction.CurrentValue);
	}
}
