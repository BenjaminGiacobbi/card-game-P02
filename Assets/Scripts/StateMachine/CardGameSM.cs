using UnityEngine;

public class CardGameSM : StateMachine
{
    [SerializeField] InputController _input = null;
    public InputController Input => _input;
    [SerializeField] PlayerController _player = null;
    public PlayerController Player => _player;
    [SerializeField] EnemyController _enemy = null;
    public EnemyController Enemy => _enemy;

    void Start()
    {
        // set starting state, already on object
        // NOTE: this allows any states, not just CardGameStates, do change
        RecordData loadData = SaveSystem.LoadRecordData();
        if(loadData != null)
        {
            Player.Wins = loadData.Wins;
            Player.Losses = loadData.Losses;
        }
        else
        {
            Player.Wins = 0;
            Player.Losses = 0;
        }
        ChangeState<MenuCardGameState>();
    }
}
