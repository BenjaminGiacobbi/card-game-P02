using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] DeckTester _deckTester = null;

    [Header("These three panels should implement IDeckView interfaces")]
    [SerializeField] Image _handDeckPanel = null;
    [SerializeField] Image _discardDeckPanel = null;
    [SerializeField] Image _mainDeckPanel = null;

    private void OnEnable()
    {
        _deckTester.CurrentHand += DisplayPlayerHand;
        _deckTester.CurrentDiscard += DisplayDiscardPile;
        _deckTester.CurrentMainDeck += DisplayMainDeck;
    }

    void DisplayPlayerHand(Deck<AbilityCard> deck)
    {
        IDeckView<AbilityCard> _handDeckView = _handDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        if (_handDeckView != null)
            _handDeckView.ShowDeck(deck);
    }

    void DisplayDiscardPile(Deck<AbilityCard> deck)
    {
        IDeckView<AbilityCard> _discardDeckView = _discardDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        if (_discardDeckView != null)
            _discardDeckView.ShowDeck(_deckTester._abilityDiscard);
    }

    void DisplayMainDeck(Deck<AbilityCard> deck)
    {
        IDeckView<AbilityCard> _mainDeckView = _mainDeckPanel.GetComponent<IDeckView<AbilityCard>>();
        if (_mainDeckView != null)
            _mainDeckView.ShowDeck(_deckTester._abilityDeck);
    }
}
