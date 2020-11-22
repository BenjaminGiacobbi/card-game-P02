[System.Serializable]
public class RecordData
{
    public int Wins;
    public int Losses;

    public RecordData(int wins, int losses)
    {
        Wins = wins;
        Losses = losses;
    }
}
