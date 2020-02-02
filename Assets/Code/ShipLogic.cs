using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipLogic : MonoBehaviour
{
	public int PartsTotal;
	[FormerlySerializedAs("PartsReceived")] public int PartsDelivered;
	public IList<PartLogic> Parts => _parts;
	private List<PartLogic> _parts;

	private GameView _view;
	
	public void Start()
	{
		_parts = FindObjectsOfType<PartLogic>().ToList();
		PartsTotal = _parts.Count;
		_view = FindObjectOfType<GameView>();
	}
	
	public void DeliverPart(PartLogic partLogic)
	{
		++PartsDelivered;

		_parts.Remove(partLogic);
		_view.PartDelivered(partLogic);

		if (PartsDelivered == PartsTotal) {
			Game.Instance.StartLevel("Victory", new GameResult {
				Type = GameResultType.Victory,
				PartsDelivered = PartsDelivered,
				PartsTotal = PartsTotal,
				Time = FindObjectOfType<GameLevel>().TimeElapsed,
			});
		}
	}
}
