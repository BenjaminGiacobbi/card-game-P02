using UnityEngine;
using UnityEngine.UI;

public class BoostDeckView : MonoBehaviour, IDeckView<BoostCard>
{
    [SerializeField] BoostCardView _boostCardPrefab = null;
    [SerializeField] Text _boostCountText = null;
    [SerializeField] PlayerController _player = null;
    [SerializeField] GameObject _mainPanel = null;
    
    private BoostCard _lastCard = null;
    private int _lastCount = 99;

    private GameObject _cardObject = null;
    private BoostCardView _mainView = null;
    private Button _mainButton = null;

    private GameObject _animObject = null;
    private BoostCardView _animView = null;

    private void Awake()
    {
        _cardObject = Instantiate(_boostCardPrefab.gameObject, transform);
        _mainView = _cardObject.GetComponent<BoostCardView>();
        _mainButton = _cardObject.GetComponent<Button>();
        _mainButton.onClick.AddListener(_player.StartBoostRoutine);
        _cardObject.transform.SetAsFirstSibling();
        _cardObject.transform.localPosition = Vector3.zero;
        _cardObject.SetActive(false);

        _animObject = Instantiate(_boostCardPrefab.gameObject, transform);
        _animView = _animObject.GetComponent<BoostCardView>();
        _animObject.transform.SetAsFirstSibling();
        _animObject.transform.position = _cardObject.transform.position;
        _animObject.SetActive(false);
    }

    // TODO this still doesn't display correctly
    public void ShowDeck(Deck<BoostCard> deck)
    {
        if (!deck.IsEmpty)
        {
            if ((_lastCard != deck.GetCard(0) && _lastCard != null) && _lastCount >= deck.Count)
            {
                _mainButton.interactable = false;
                _animObject.transform.SetAsLastSibling();
                _animView.Display(deck.GetCard(0));
                _animObject.SetActive(true);
                RectTransform rect = _cardObject.GetComponent<RectTransform>();
                Vector3 newPos = new Vector3(rect.transform.position.x, rect.transform.position.y + rect.rect.height / 2, 0);
                LeanTween.move(_animObject, newPos, 0.2f).setOnComplete(FinishFlipAnimation);
                _mainView.Display(deck.TopItem);
            }
            // only when a card is removed from the deck
            else if (_lastCount < deck.Count)
            {
                _animObject.transform.SetAsFirstSibling();
                _animObject.SetActive(true);
                _animView.Display(deck.GetCard(0));
                Vector2 scale = _cardObject.transform.localScale;
                LeanTween.scale(_cardObject, Vector2.zero, 0.3f).setOnComplete(
                    () => { LeanTween.scale(_cardObject, scale, 0f).setOnComplete(
                        () => { _mainView.Display(deck.GetCard(0)); }); }); 
            }
            else
            {
                _mainView.Display(deck.GetCard(0));
            }

            _cardObject.SetActive(true);
            _lastCard = deck.GetCard(0);
            _lastCount = deck.Count;
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
        _mainButton.interactable = true;
    }

    private void FinishVanishAnimation()
    {
        _animObject.SetActive(true);
    }

    public void HideDeck()
    {
        _cardObject.SetActive(false);
        _mainView.EmptyDisplay();
    }
}
