using UnityEngine;
using UnityEngine.UI;

public class PlayDeckView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] Text _deckCountText = null;
    [SerializeField] Image _cardBackImage = null;

    private void Start()
    {
        _deckCountText.text = "0";
        _cardBackImage.gameObject.SetActive(false);
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        _cardBackImage.gameObject.SetActive(true);
        _deckCountText.text = deck.Count.ToString();
    }

    public void HideDeck()
    {
        _cardBackImage.gameObject.SetActive(false);
    }
}
