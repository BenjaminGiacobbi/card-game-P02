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
    [SerializeField] Text _enemyThinkingText = null;
    [SerializeField] Text _playerTurnText = null;

    [SerializeField] BoostCardView _boostCardPrefab = null;
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    [SerializeField] GameObject _menuPanel = null;
    [SerializeField] GameObject _setupPanel = null;
    [SerializeField] GameObject _boostStepPanel = null;
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
    IDeckView<BoostCard> _boostStepView = null;

    private GameObject _abilityObject;
    private GameObject _boostObject;
    int _playerTurnCount = 0;

    private void OnEnable()
    {
        MenuCardGameState.EnteredMenu += ShowMenu;
        MenuCardGameState.ExitedMenu += HideMenu;
        SetupCardGameState.StartedSetup += ShowSetupGraphics;
        SetupCardGameState.StartedSetup -= ResetTurnDisplay;
        SetupCardGameState.StartedSetup += HideMainPanel;
        SetupCardGameState.EndedSetup += HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep += ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep += HideBoostStep;
        BoostStepCardGameState.BoostCardChosen -= AnimateBoostStep;
        PlayerTurnCardGameState.StartedPlayerTurn += ShowMainPanel;
        PlayerTurnCardGameState.StartedPlayerTurn += UpdateTurnDisplay;
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
        _player.SelectedBoostCard += ShowSelectedGraphic;
        _player.EndedSelection += HideSelectedGraphic;
    }

    private void OnDisable()
    {
        MenuCardGameState.EnteredMenu -= ShowMenu;
        MenuCardGameState.ExitedMenu -= HideMenu;
        SetupCardGameState.StartedSetup -= ShowSetupGraphics;
        SetupCardGameState.StartedSetup -= ResetTurnDisplay;
        SetupCardGameState.StartedSetup -= HideMainPanel;
        SetupCardGameState.EndedSetup -= HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep -= ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep -= HideBoostStep;
        BoostStepCardGameState.BoostCardChosen -= AnimateBoostStep;
        PlayerTurnCardGameState.StartedPlayerTurn -= ShowMainPanel;
        PlayerTurnCardGameState.StartedPlayerTurn -= UpdateTurnDisplay;
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
        _player.SelectedBoostCard -= ShowSelectedGraphic;
        _player.EndedSelection -= HideSelectedGraphic;
    }

    private void Awake()
    {
        _handDeckView = _handDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _discardDeckView = _discardDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _mainDeckView = _mainDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _boostDeckView = _boostDeckPanel.GetComponent<IDeckView<BoostCard>>();
        _boostStepView = _boostStepPanel.GetComponent<IDeckView<BoostCard>>();
    }

    private void Start()
    {
        _abilityObject = Instantiate(_abilityCardPrefab.gameObject, transform);
        _abilityObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        _abilityObject.GetComponent<Button>().interactable = false;
        _boostObject = Instantiate(_boostCardPrefab.gameObject, transform);
        _boostObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        _boostObject.GetComponent<Button>().interactable = false;
        _abilityObject.SetActive(false);
        _boostObject.SetActive(false);
        _playerTurnText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_abilityObject.activeSelf == true)
        {
            _abilityObject.transform.position = Input.mousePosition;
        }
        if (_boostObject.activeSelf == true)
        {
            _boostObject.transform.position = Input.mousePosition;
        }
    }

    private void ResetTurnDisplay()
    {
        _playerTurnText.text = "Player Turn: 0";
    }

    private void UpdateTurnDisplay()
    {
        _playerTurnCount++;
        _playerTurnText.text = "Player Turn : " + _playerTurnCount;
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
        
        _actionText.text = "Actions: " + currentActions;
    }

    private void UpdateDefenseDisplay(float currentDefense)
    {
        _defenseText.text = "Defense: " +  (1 / currentDefense * 100) + "%";
    }

    private void UpdateHealthDisplay(int currentHealth)
    {
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
        _boostStepPanel.SetActive(true);
        _mainPlayerPanel.SetActive(true);
    }

    private void HideSetupGraphics()
    {
        _setupPanel.SetActive(false);
        _boostStepPanel.SetActive(false);
        _mainPlayerPanel.SetActive(false);
    }

    private void ShowBoostStep()
    {
        _boostStepPanel.SetActive(true);
        _boostStepPanel.GetComponent<IDeckView<BoostCard>>()?.ShowDeck(_player.BoostDeck);
    }

    private void HideBoostStep()
    {
        _boostStepPanel.SetActive(false);
    }

    private void AnimateBoostStep()
    {
        _boostStepView.ShowDeck(_player.BoostDeck);
    }

    private void ShowMainPanel()
    {
        _mainPlayerPanel.SetActive(true);
        _playerTurnText.gameObject.SetActive(true);
    }

    private void HideMainPanel()
    {
        _mainPlayerPanel.SetActive(false);
        _playerTurnText.gameObject.SetActive(false);
    }

    private void OnEnemyTurnBegan()
    {
        _enemyThinkingText.gameObject.SetActive(true);
    }

    private void OnEnemyTurnEnded()
    {
        _enemyThinkingText.gameObject.SetActive(false);
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

    private void ShowSelectedGraphic(Card selectedCard)
    {
        BoostCard boostCard = selectedCard as BoostCard;
        if(boostCard != null)
        {
            _boostObject.GetComponent<BoostCardView>().Display(boostCard);
            _boostObject.SetActive(true);
        }

        AbilityCard abilityCard = selectedCard as AbilityCard;
        if(abilityCard != null)
        {
            _abilityObject.GetComponent<AbilityCardView>().Display(abilityCard);
            _abilityObject.SetActive(true);
        }
    }

    private void HideSelectedGraphic()
    {
        _abilityObject.SetActive(false);
        _boostObject.SetActive(false);
    }
}
