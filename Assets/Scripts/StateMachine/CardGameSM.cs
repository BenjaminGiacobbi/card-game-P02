using UnityEngine;

public class CardGameSM : StateMachine
{
    [SerializeField] InputController _input = null;
    public InputController Input => _input;
    [SerializeField] CardGameController _player = null;
    public CardGameController Player => _player;
    [SerializeField] CardGameController _enemy = null;
    public CardGameController Enemy => _enemy;

    void Start()
    {
        // set starting state, already on object
        // NOTE: this allows any states, not just CardGameStates, do change
        ChangeState<MenuCardGameState>();
    }
}
