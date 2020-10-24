using UnityEngine;

public class CardGameSM : StateMachine
{
    [SerializeField] InputController _input = null;
    public InputController Input => _input;
    [SerializeField] PlayerController _player = null;
    public PlayerController Player => _player;

    void Start()
    {
        // set starting state, already on object
        // NOTE: this allows any states, not just CardGameStates, do change
        ChangeState<MenuCardGameState>();
    }

    private void Update()
    {
            
    }
}
