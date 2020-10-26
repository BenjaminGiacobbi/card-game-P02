using UnityEngine;
using UnityEngine.UI;

public class PlayDeckView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] Text _deckCountText = null;
    [SerializeField] Button _cardButton = null;
    [SerializeField] PlayerController _player = null;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        _deckCountText.text = "0";
        _cardButton.onClick.AddListener(_player.Draw);
        _cardButton.gameObject.SetActive(false);
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        _cardButton.gameObject.SetActive(true);
        _deckCountText.text = deck.Count.ToString();
    }

    public void HideDeck()
    {
        _cardButton.gameObject.SetActive(false);
    }
}
