using UnityEngine;
using UnityEngine.UI;

public class BoostDeckView : MonoBehaviour, IDeckView<BoostCard>
{
    [SerializeField] BoostCardView _boostCardPrefab = null;
    [SerializeField] Text _boostCountText = null;
    [SerializeField] PlayerController _player = null;
    [SerializeField] GameObject _mainPanel = null;
    private GameObject _animObject = null;
    private GameObject _cardObject = null;
    private BoostCard _lastCard = null;

    private void Start()
    {
        _cardObject = Instantiate(_boostCardPrefab.gameObject, transform);
        _cardObject.transform.SetSiblingIndex(0);
        _cardObject.transform.localPosition = Vector3.zero;
        _cardObject.GetComponent<Button>()?.onClick.AddListener(_player.StartBoostRoutine);
        _cardObject.SetActive(false);
        gameObject.SetActive(false);

        _animObject = Instantiate(_boostCardPrefab.gameObject, _mainPanel.transform);
        _animObject.transform.SetAsFirstSibling();
        _animObject.transform.position = _cardObject.transform.position;
        _animObject.SetActive(false);
    }

    public void ShowDeck(Deck<BoostCard> deck)
    {
        gameObject.SetActive(true);
        if (!deck.IsEmpty)
        {

            if(_lastCard != deck.GetCard(0) && _lastCard != null)
            {
                _cardObject.GetComponent<Button>().interactable = false;
                _animObject.transform.SetAsLastSibling();
                _animObject.GetComponent<BoostCardView>().Display(deck.GetCard(0));
                _animObject.SetActive(true);
                Vector3 newPos = new Vector3(_cardObject.transform.position.x, _cardObject.transform.position.y + 100, 0);
                LeanTween.move(_animObject, newPos, 0.2f).setOnComplete(FinishFlipAnimation);
            }
            _cardObject.GetComponent<BoostCardView>().Display(deck.TopItem);
            _cardObject.SetActive(true);
            _lastCard = deck.GetCard(0);
        }
        else
        {
            _cardObject.SetActive(false);
        }
        _boostCountText.text = deck.Count.ToString();
    }

    private void FinishFlipAnimation()
    {
        _animObject.transform.SetAsFirstSibling();
        LeanTween.move(_animObject, _cardObject.transform.position, 0.2f).setOnComplete(
            () => { _animObject.SetActive(false); });
        _cardObject.GetComponent<Button>().interactable = true;
    }

    public void HideDeck()
    {
        gameObject.SetActive(false);
        _cardObject.GetComponent<BoostCardView>()?.EmptyDisplay();
    }
}
