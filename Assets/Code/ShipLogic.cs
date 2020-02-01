using UnityEngine;

public class ShipLogic : MonoBehaviour
{
	public int PartsTotal;
	public int PartsReceived;
	
	private GameView _view;
	
	public void Start()
	{
		PartLogic[] parts = FindObjectsOfType<PartLogic>();
		PartsTotal = parts.Length;
		_view = FindObjectOfType<GameView>();
		_view.SetPartsLeft(PartsReceived, PartsTotal);
	}
	
	public void DeliverPart(PartLogic partLogic)
	{
		++PartsReceived;

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
