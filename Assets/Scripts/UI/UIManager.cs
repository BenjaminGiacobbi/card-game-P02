using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // TODO this is a major mess, pffload these into individual scripts on each menu panel that derive from an abstract class

    [SerializeField] PlayerController _player = null;
    [SerializeField] Text _playerTurnText = null;

    [SerializeField] BoostCardView _boostCardPrefab = null;
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    [SerializeField] GameObject _menuPanel = null;
    [SerializeField] PanelUI _setupPanel = null;
    [SerializeField] PanelUI _boostStepPanel = null;
    [SerializeField] PanelUI _mainPlayerPanel = null;
    [SerializeField] PanelUI _enemyPanel = null;
    [SerializeField] PanelUI _resultsPanel = null;
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
    private Vector3 _point1 = Vector3.zero;
    private Vector3 _point2 = Vector3.zero;
    int _playerTurnCount = 0;

    private void OnEnable()
    {
        MenuCardGameState.EnteredMenu += ShowMenu;
        MenuCardGameState.ExitedMenu += HideMenu;
        SetupCardGameState.StartedSetup += ShowSetupGraphics;
        SetupCardGameState.StartedSetup += ResetTurnDisplay;
        SetupCardGameState.StartedSetup += HideMainPanel;
        SetupCardGameState.EndedSetup += HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep += ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep += HideBoostStep;
        BoostStepCardGameState.PlayerBoosted += AnimateBoostStep;
        BoostStepCardGameState.EnemyBoosted += DisplayEnemyBoost;
        PlayerTurnCardGameState.StartedPlayerTurn += ShowMainPanel;
        PlayerTurnCardGameState.StartedPlayerTurn += UpdateTurnDisplay;
        PlayerTurnCardGameState.EndedPlayerTurn += HideMainPanel;
        EnemyTurnCardGameState.EnemyTurnBegan += OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded += OnEnemyTurnEnded;
        TurnResultsCardGameState.ResultsBegan += OnResultsBegan;
        TurnResultsCardGameState.ResultsEnded += OnResultsEnded;
        PlayerWinCardGameState.StartedWinState += ShowWinPanel;
        PlayerWinCardGameState.EndedWinState += HideWinPanel;
        PlayerLoseCardGameState.StartedLoseState += ShowLosePanel;
        PlayerLoseCardGameState.EndedLoseState += HideLosePanel;
        _player.CurrentHand += DisplayPlayerHand;
        _player.CurrentMainDeck += DisplayMainDeck;
        _player.CurrentDiscard += DisplayDiscardPile;
        _player.CurrentBoostDeck += DisplayBoostDeck;
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
        BoostStepCardGameState.PlayerBoosted -= AnimateBoostStep;
        BoostStepCardGameState.EnemyBoosted -= DisplayEnemyBoost;
        PlayerTurnCardGameState.StartedPlayerTurn -= ShowMainPanel;
        PlayerTurnCardGameState.StartedPlayerTurn -= UpdateTurnDisplay;
        PlayerTurnCardGameState.EndedPlayerTurn -= HideMainPanel;
        EnemyTurnCardGameState.EnemyTurnBegan -= OnEnemyTurnBegan;
        EnemyTurnCardGameState.EnemyTurnEnded -= OnEnemyTurnEnded;
        TurnResultsCardGameState.ResultsBegan -= OnResultsBegan;
        TurnResultsCardGameState.ResultsEnded-= OnResultsEnded;
        PlayerWinCardGameState.StartedWinState -= ShowWinPanel;
        PlayerWinCardGameState.EndedWinState -= HideWinPanel;
        PlayerLoseCardGameState.StartedLoseState -= ShowLosePanel;
        PlayerLoseCardGameState.EndedLoseState -= HideLosePanel;
        _player.CurrentHand -= DisplayPlayerHand;
        _player.CurrentMainDeck -= DisplayMainDeck;
        _player.CurrentDiscard -= DisplayDiscardPile;
        _player.CurrentBoostDeck -= DisplayBoostDeck;
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
        _playerTurnText.text = "Turn: 0";
    }

    private void UpdateTurnDisplay()
    {
        _playerTurnCount++;
        _playerTurnText.text = "Turn : " + _playerTurnCount;
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
        _boostStepView.ShowDeck(deck);
    }

    private void ShowMenu()
    {
        _menuPanel.SetActive(true);
        _setupPanel.gameObject.SetActive(true);
    }

    private void HideMenu()
    {
        _menuPanel.SetActive(false);
        _setupPanel.gameObject.SetActive(false);
    }

    private void ShowSetupGraphics()
    {
        _setupPanel.OpenAnimation();
        _boostStepPanel.gameObject.SetActive(true);
        _mainPlayerPanel.gameObject.SetActive(true);
    }

    private void HideSetupGraphics()
    {
        _boostStepPanel.gameObject.SetActive(false);
        _mainPlayerPanel.gameObject.SetActive(false);
        _setupPanel.CloseAnimation();
    }

    private void ShowBoostStep()
    {
        _boostStepView.ShowDeck(_player.BoostDeck);
        _boostStepPanel.OpenAnimation();
    }

    private void HideBoostStep()
    {
        _boostStepPanel.CloseAnimation();
    }

    private void AnimateBoostStep()
    {
        _boostStepView.ShowDeck(_player.BoostDeck);
    }

    private void DisplayEnemyBoost(bool choice)
    {
        BoostPanelUI boostUI = _boostStepPanel as BoostPanelUI;
        if(boostUI)
            boostUI.DisplayEnemyBoostChoice(choice);
    }

    private void ShowMainPanel()
    {
        _mainPlayerPanel.OpenAnimation();
    }

    private void HideMainPanel()
    {
        _mainPlayerPanel.CloseAnimation();
        _playerTurnText.text = "Turn: ";
    }

    private void OnEnemyTurnBegan()
    {
        _enemyPanel.OpenAnimation();
    }

    private void OnEnemyTurnEnded()
    {
        _enemyPanel.CloseAnimation();
    }

    private void OnResultsBegan()
    {
        _resultsPanel.OpenAnimation();
    }

    private void OnResultsEnded()
    {
        _resultsPanel.CloseAnimation();
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
