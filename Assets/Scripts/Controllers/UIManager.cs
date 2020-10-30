using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // TODO this is a major mess, pffload these into individual scripts on each menu panel that derive from an abstract class

    [SerializeField] PlayerController _player = null;
    [SerializeField] Text _actionText = null;
    [SerializeField] Text _healthText = null;
    [SerializeField] Text _defenseText = null;
    [SerializeField] Text _enemyThinkingTextUI = null;

    [SerializeField] GameObject _testObject = null;
    [SerializeField] GameObject _menuPanel = null;
    [SerializeField] GameObject _setupPanel = null;
    [SerializeField] GameObject _boostPanel = null;
    [SerializeField] GameObject _mainPlayerPanel = null;
    [SerializeField] GameObject _losePanel = null;
    [SerializeField] GameObject _winPanel = null;

    [Header("These four panels should implement IDeckView interfaces")]
    [SerializeField] Image _handDeckPanel = null;
    [SerializeField] Image _discardDeckPanel = null;
    [SerializeField] Image _mainDeckPanel = null;
    [SerializeField] Image _boostDeckPanel = null;

    IDeckView<AbilityCard> _handDeckView = null;
    IDeckView<AbilityCard> _discardDeckView = null;
    IDeckView<AbilityCard> _mainDeckView = null;
    IDeckView<BoostCard> _boostDeckView = null;

    private void OnEnable()
    {
        MenuCardGameState.EnteredMenu += ShowMenu;
        MenuCardGameState.ExitedMenu += HideMenu;
        SetupCardGameState.StartedSetup += ShowSetupGraphics;
        SetupCardGameState.StartedSetup += HideMainPanel;
        SetupCardGameState.EndedSetup += HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep += ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep += HideBoostStep;
        PlayerTurnCardGameState.StartedPlayerTurn += ShowMainPanel;
        PlayerTurnCardGameState.EndedPlayerTurn += HideMainPanel;
        EnemyTurnCardGameState.EnemyTurnBegan += OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded += OnEnemyTurnEnded;
        PlayerWinCardGameState.StartedWinState += ShowWinPanel;
        PlayerWinCardGameState.EndedWinState += HideWinPanel;
        PlayerLoseCardGameState.StartedLoseState += ShowLosePanel;
        PlayerLoseCardGameState.EndedLoseState += HideLosePanel;
        _player.CurrentHand += DisplayPlayerHand;
        _player.CurrentMainDeck += DisplayMainDeck;
        _player.CurrentDiscard += DisplayDiscardPile;
        _player.CurrentBoostDeck += DisplayBoostDeck;
        _player.ActionsChanged += UpdateActionsDisplay;
        _player.HealthSet += UpdateHealthDisplay;
        _player.DefenseChanged += UpdateDefenseDisplay;
        _player.SelectedAbilityCard += ShowSelectedGraphic;
        _player.EndedSelection += HideSelectedGraphic;
    }

    private void OnDisable()
    {
        MenuCardGameState.EnteredMenu -= ShowMenu;
        MenuCardGameState.ExitedMenu -= HideMenu;
        SetupCardGameState.StartedSetup -= ShowSetupGraphics;
        SetupCardGameState.StartedSetup -= HideMainPanel;
        SetupCardGameState.EndedSetup -= HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep -= ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep -= HideBoostStep;
        PlayerTurnCardGameState.StartedPlayerTurn -= ShowMainPanel;
        PlayerTurnCardGameState.EndedPlayerTurn -= HideMainPanel;
        EnemyTurnCardGameState.EnemyTurnBegan -= OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded -= OnEnemyTurnEnded;
        PlayerWinCardGameState.StartedWinState -= ShowWinPanel;
        PlayerWinCardGameState.EndedWinState -= HideWinPanel;
        PlayerLoseCardGameState.StartedLoseState -= ShowLosePanel;
        PlayerLoseCardGameState.EndedLoseState -= HideLosePanel;
        _player.CurrentHand -= DisplayPlayerHand;
        _player.CurrentMainDeck -= DisplayMainDeck;
        _player.CurrentDiscard -= DisplayDiscardPile;
        _player.CurrentBoostDeck -= DisplayBoostDeck;
        _player.ActionsChanged -= UpdateActionsDisplay;
        _player.HealthSet -= UpdateHealthDisplay;
        _player.DefenseChanged -= UpdateDefenseDisplay;
        _player.SelectedAbilityCard -= ShowSelectedGraphic;
        _player.EndedSelection -= HideSelectedGraphic;
    }

    private void Awake()
    {
        _handDeckView = _handDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _discardDeckView = _discardDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _mainDeckView = _mainDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _boostDeckView = _boostDeckPanel.GetComponent<IDeckView<BoostCard>>();
    }

    private void Start()
    {
        _testObject = Instantiate(_testObject, transform);
        _testObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        _testObject.transform.SetAsLastSibling();
        _testObject.SetActive(false);
    }

    private void Update()
    {
        if (_testObject.activeSelf == true)
        {
            _testObject.transform.position = Input.mousePosition;
        }
    }

    private void DisplayPlayerHand(Deck<AbilityCard> deck)
    {
        _handDeckView.ShowDeck(deck);
    }

    private void DisplayDiscardPile(Deck<AbilityCard> deck)
    {
        _discardDeckView.ShowDeck(deck);
    }

    private void DisplayMainDeck(Deck<AbilityCard> deck)
    {   
         _mainDeckView.ShowDeck(deck);
    }

    private void DisplayBoostDeck(Deck<BoostCard> deck)
    {
        _boostDeckView.ShowDeck(deck);
    }

    private void UpdateActionsDisplay(int currentActions)
    {
        
        Debug.Log("Current Actions: " + currentActions);
        _actionText.text = "Actions: " + currentActions;
    }

    private void UpdateDefenseDisplay(float currentDefense)
    {
        Debug.Log("Updating Defense");
        _defenseText.text = "Defense: " + currentDefense + "%";
    }

    private void UpdateHealthDisplay(int currentHealth)
    {
        Debug.Log("Updating Health");
        _healthText.text = "Health: " + currentHealth;
    }

    private void ShowMenu()
    {
        _menuPanel.SetActive(true);
    }

    private void HideMenu()
    {
        _menuPanel.SetActive(false);
    }

    private void ShowSetupGraphics()
    {
        _setupPanel.SetActive(true);
    }

    private void HideSetupGraphics()
    {
        _setupPanel.SetActive(false);
    }

    private void ShowBoostStep()
    {
        _boostPanel.SetActive(true);
        _boostPanel.GetComponent<IDeckView<BoostCard>>()?.ShowDeck(_player._boostDeck);
    }

    private void HideBoostStep()
    {
        _boostPanel.SetActive(false);
    }

    private void ShowMainPanel()
    {
        _mainPlayerPanel.SetActive(true);
    }

    private void HideMainPanel()
    {
        _mainPlayerPanel.SetActive(false);
    }

    private void OnEnemyTurnBegan()
    {
        _enemyThinkingTextUI.gameObject.SetActive(true);
    }

    private void OnEnemyTurnEnded()
    {
        _enemyThinkingTextUI.gameObject.SetActive(false);
    }

    private void ShowWinPanel()
    {
        _winPanel.SetActive(true);
    }

    private void HideWinPanel()
    {
        _winPanel.SetActive(false);
    }

    private void ShowLosePanel()
    {
        _losePanel.SetActive(true);
    }

    private void HideLosePanel()
    {
        _losePanel.SetActive(false);
    }

    private void ShowSelectedGraphic(AbilityCard card)
    {
        _testObject.SetActive(true);
    }

    private void HideSelectedGraphic()
    {
        _testObject.SetActive(false);
    }
}
