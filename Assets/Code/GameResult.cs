public enum GameResultType
{
	Victory,
	BatteryDepleted,
	Destroyed,
}

public class GameResult
{
	public GameResultType Type;
	public float Time;
}
