using UnityEngine;

public class ShipLogic : MonoBehaviour
{
	public int PartsTotal;
	public int PartsReceived;
	
	private GameView _view;
	private GameLevel _level;

	public void Start()
	{
		PartLogic[] parts = FindObjectsOfType<PartLogic>();
		PartsTotal = parts.Length;
		_view = FindObjectOfType<GameView>();
		_view.SetPartsLeft(PartsReceived, PartsTotal);
		_level = FindObjectOfType<GameLevel>();
	}
	
	public void DeliverPart(PartLogic partLogic)
	{
		++PartsReceived;

		_level.BroadcastPartDelivered(partLogic);

		_view.SetPartsLeft(PartsReceived, PartsTotal);
		
		if (PartsReceived >= PartsTotal) {
			Game.Instance.StartLevel("Victory", new GameResult {
				Type = GameResultType.Victory,
				PartsReceived = PartsReceived,
				PartsTotal = PartsTotal,
				Time = FindObjectOfType<GameLevel>().TimeElapsed,
			});
		}
	}
}
