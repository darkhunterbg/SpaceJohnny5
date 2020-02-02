using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipLogic : MonoBehaviour
{
	[System.Serializable]
	public class PartVisual
	{
		[Range(0f, 1f)]
		public float RequiredProgress = 0;
		public GameObject Visual;

		public void Update(int currentParts, int partsTotal) {
			Visual.SetActive(RequiredProgress <= (float) currentParts / partsTotal);
		}
	}

	public int PartsTotal;
	[FormerlySerializedAs("PartsReceived")] public int PartsDelivered;
	public IList<PartLogic> Parts => _parts;
	private List<PartLogic> _parts;

	[SerializeField] private PartVisual[] PartVisuals;

	private GameView _view;
	
	public void Start()
	{
		_parts = FindObjectsOfType<PartLogic>().ToList();
		PartsTotal = _parts.Count;
		_view = FindObjectOfType<GameView>();
		UpdateVisuals();
	}
	
	public void DeliverPart(PartLogic partLogic)
	{
		++PartsDelivered;

		_parts.Remove(partLogic);
		_view.PartDelivered(partLogic);

		UpdateVisuals();

		if (PartsDelivered == PartsTotal) {
			Game.Instance.StartLevel("Victory", new GameResult {
				Type = GameResultType.Victory,
				PartsDelivered = PartsDelivered,
				PartsTotal = PartsTotal,
				Time = FindObjectOfType<GameLevel>().TimeElapsed,
			});
		}
	}

	private void UpdateVisuals ()
	{
		foreach (var visual in PartVisuals) {
			visual.Update(PartsDelivered, PartsTotal);
		}
	}
}
