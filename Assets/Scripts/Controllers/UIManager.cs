using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // TODO this is a major mess, probably offload this into individual state graphical scripts?

    [SerializeField] DeckTester _deckTester = null;
    [SerializeField] PlayerController _player = null;
    [SerializeField] Text _energyText = null;

    [SerializeField] GameObject _menuPanel = null;
    [SerializeField] GameObject _setupPanel = null;
    [SerializeField] GameObject _boostPanel = null;

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
        SetupCardGameState.EndedSetup += HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep += ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep += HideBoostStep;
        _deckTester.CurrentHand += DisplayPlayerHand;
        _deckTester.CurrentMainDeck += DisplayMainDeck;
        _deckTester.CurrentDiscard += DisplayDiscardPile;
        _deckTester.CurrentBoostDeck += DisplayBoostDeck;
        _player.EnergyChanged += UpdateEnergyDisplay;
    }

    private void OnDisable()
    {
        MenuCardGameState.EnteredMenu -= ShowMenu;
        MenuCardGameState.ExitedMenu -= HideMenu;
        SetupCardGameState.StartedSetup -= ShowSetupGraphics;
        SetupCardGameState.EndedSetup -= HideSetupGraphics;
        BoostStepCardGameState.StartedBoostStep -= ShowBoostStep;
        BoostStepCardGameState.EndedBoostStep -= HideBoostStep;
        _deckTester.CurrentHand -= DisplayPlayerHand;
        _deckTester.CurrentMainDeck -= DisplayMainDeck;
        _deckTester.CurrentDiscard -= DisplayDiscardPile;
        _deckTester.CurrentBoostDeck -= DisplayBoostDeck;
        _player.EnergyChanged -= UpdateEnergyDisplay;
    }

    private void Awake()
    {
        _menuPanel.SetActive(false);
        _setupPanel.SetActive(false);
        _handDeckView = _handDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _discardDeckView = _discardDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _mainDeckView = _mainDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        _boostDeckView = _boostDeckPanel.GetComponent<IDeckView<BoostCard>>();
    }

    private void Start()
    {
        UpdateEnergyDisplay(_player.PlayerEnergy);
    }

    private void DisplayPlayerHand(Deck<AbilityCard> deck)
    {
        _handDeckView.ShowDeck(deck);
    }

    private void HidePlayerHand()
    {
        _handDeckView.HideDeck();
    }

    private void DisplayDiscardPile(Deck<AbilityCard> deck)
    {
        _discardDeckView.ShowDeck(deck);
    }

    private void HideDiscardPile()
    {
        _discardDeckView.HideDeck();
    }

    private void DisplayMainDeck(Deck<AbilityCard> deck)
    {   
         _mainDeckView.ShowDeck(deck);
    }

    private void HideMainDeck()
    {
        _mainDeckView.HideDeck();
    }

    private void DisplayBoostDeck(Deck<BoostCard> deck)
    {
        _boostDeckView.ShowDeck(deck);
    }

    private void HideBoostDeck()
    {
        _boostDeckView.HideDeck();
    }

    private void UpdateEnergyDisplay(int currentEnergy)
    {
        _energyText.text = "Energy: " + currentEnergy;
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
        HidePlayerHand();
        HideDiscardPile();
        HideBoostDeck();
        HideMainDeck();
        _boostPanel.SetActive(true);
        _boostPanel.GetComponent<IDeckView<BoostCard>>()?.ShowDeck(_deckTester._boostDeck);
    }

    private void HideBoostStep()
    {
        _boostPanel.SetActive(false);
    }
}
