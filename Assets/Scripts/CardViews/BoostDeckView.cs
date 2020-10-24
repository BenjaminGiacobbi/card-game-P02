using UnityEngine;
using UnityEngine.UI;

public class BoostDeckView : MonoBehaviour, IDeckView<BoostCard>
{
    [SerializeField] BoostCardView _boostCardPrefab = null;
    private GameObject _cardObject = null;

    private void Start()
    {
        _cardObject = Instantiate(_boostCardPrefab.gameObject, transform);
        _cardObject.transform.SetSiblingIndex(0);
        _cardObject.transform.localPosition = Vector3.zero;
        _cardObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowDeck(Deck<BoostCard> deck)
    {
        gameObject.SetActive(true);
        Debug.Log("Test 2");
        Debug.Log(deck.IsEmpty);
        if (!deck.IsEmpty)
        {
            Debug.Log("test3");
            BoostCardView cardView = _cardObject.GetComponent<BoostCardView>();
            Debug.Log(cardView);
            cardView.Display(deck.TopItem);
            _cardObject.SetActive(true);
        }
        else
        {
            _cardObject.SetActive(false);
        }
    }

    public void HideDeck()
    {
        gameObject.SetActive(false);
        _cardObject.GetComponent<BoostCardView>()?.EmptyDisplay();
    }
}
