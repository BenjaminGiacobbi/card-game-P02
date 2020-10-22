using UnityEngine;
using UnityEngine.UI;

public class DiscardDeckView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    [SerializeField] Text _discardCountText = null;

    private GameObject _cardObject = null;

    private void Start()
    {
        _cardObject = Instantiate(_abilityCardPrefab.gameObject, transform);
        _cardObject.transform.SetSiblingIndex(0);
        _cardObject.transform.localPosition = Vector3.zero;
        _cardObject.SetActive(false);
        _discardCountText.text = "0";
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        if (!deck.IsEmpty)
        {
            _cardObject.GetComponent<AbilityCardView>()?.Display(deck.TopItem);
            _cardObject.SetActive(true);
        }
        else
        {
            _cardObject.SetActive(false);
        }
        _discardCountText.text = deck.Count.ToString();
    }

    public void HideDeck()
    {
        _cardObject.SetActive(false);
        _cardObject.GetComponent<AbilityCardView>()?.EmptyDisplay();
    }
}
