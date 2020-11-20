using UnityEngine;
using UnityEngine.UI;

public class BoostDeckView : MonoBehaviour, IDeckView<BoostCard>
{
    public float AnimationTime
    { get { return _animationTime; } }

    [SerializeField] float _animationTime = 0.15f;
    [SerializeField] BoostCardView _boostCardPrefab = null;
    [SerializeField] Text _boostCountText = null;
    [SerializeField] PlayerController _player = null;
    [SerializeField] GameObject _mainPanel = null;
    [SerializeField] bool _interactable = true;
    
    private BoostCard _lastCard = null;
    private int _lastCount = 99;

    private BoostCardView _mainView = null;
    private BoostCardView _animView = null;
    private Button _mainButton = null;

    private void Awake()
    {
        _mainView = _boostCardPrefab.GetComponent<BoostCardView>();
        _boostCardPrefab.transform.SetAsFirstSibling();
        _boostCardPrefab.transform.localPosition = Vector3.zero;
        _boostCardPrefab.gameObject.SetActive(false);

        _mainButton = _boostCardPrefab.GetComponent<Button>();
        _mainButton.onClick.AddListener(_player.StartBoostRoutine);
        if (!_interactable)
            SetNotInteractable(_mainButton);

        // sets animation object and disables button - TODO this could be simplified with a simple visual prefab
        _animView = Instantiate(_boostCardPrefab, transform);
        SetNotInteractable(_animView.GetComponent<Button>());
        
        _animView.transform.SetAsFirstSibling();
        _animView.transform.position = _boostCardPrefab.transform.position;
        _animView.gameObject.SetActive(false);

        _lastCard = new BoostCard(null);
    }

    // TODO this still doesn't display correctly
    public void ShowDeck(Deck<BoostCard> deck)
    {
        if (!deck.IsEmpty)
        {
            if (_lastCard != deck.TopItem && _lastCount == deck.Count)
            {
                if(_interactable)
                    _mainButton.interactable = false;
                _animView.transform.SetAsLastSibling();
                _animView.Display(deck.GetCard(0));
                _animView.gameObject.SetActive(true);
                RectTransform rect = _boostCardPrefab.GetComponent<RectTransform>();
                Vector3 newPos = new Vector3(rect.transform.position.x, rect.transform.position.y + rect.rect.height / 2, 0);
                LeanTween.move(_animView.gameObject, newPos, AnimationTime/2).setOnComplete(FinishFlipAnimation);
                _mainView.Display(deck.TopItem);
            }
            // only when a card is removed from the deck
            else if (_lastCard != deck.TopItem && _lastCount > deck.Count)
            {
                _animView.transform.SetAsFirstSibling();
                _animView.gameObject.SetActive(true);
                _animView.Display(deck.TopItem);
                Vector2 scale = _boostCardPrefab.transform.localScale;
                LeanTween.scale(_boostCardPrefab.gameObject, Vector2.zero, AnimationTime).setOnComplete(
                    () => { FinishVanishAnimation(deck.TopItem, scale); }); 
            }
            else if (_lastCard == deck.TopItem)
            {
                _mainView.Display(deck.TopItem);
            }

            _boostCardPrefab.gameObject.SetActive(true);
            _lastCard = deck.TopItem;
            _lastCount = deck.Count;
        }
        else
        {
            _boostCardPrefab.gameObject.SetActive(false);
        }
        _boostCountText.text = deck.Count.ToString();
    }

    private void FinishFlipAnimation()
    {
        _animView.transform.SetAsFirstSibling();
        LeanTween.move(_animView.gameObject, _boostCardPrefab.transform.position, AnimationTime/2).setOnComplete(
            () => { _animView.gameObject.SetActive(false); });
        if(_interactable)
            _mainButton.interactable = true;
    }

    private void FinishVanishAnimation(BoostCard displayItem, Vector2 displayScale)
    {
        _boostCardPrefab.gameObject.SetActive(false);
        _mainView.Display(displayItem);
        LeanTween.scale(_boostCardPrefab.gameObject, displayScale, 0f).setOnComplete(
            () => { _boostCardPrefab.gameObject.SetActive(true); });
    }

    public void HideDeck()
    {
        _boostCardPrefab.gameObject.SetActive(false);
        _mainView.EmptyDisplay();
    }

    private void SetNotInteractable(Button button)
    {
        button.transition = Selectable.Transition.None;
        button.interactable = false;
    }
}
