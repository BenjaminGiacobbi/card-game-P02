using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridLayoutGroup))]
public class PlayerHandView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    [SerializeField] Transform _deckPosition = null;
    [SerializeField] Canvas _mainCanvas = null;
    [SerializeField] PlayerController _player = null;
    [SerializeField] AudioClip _drawAudio = null;
    private HandItem[] _handItems = null;
    private GridLayoutGroup _gridLayout = null;
    private int _lastCount = 0;
    private GameObject _animObject = null;
    bool startBool = false;
    Vector2 _animStartSize = Vector2.zero;

    private void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        
    }

    private void Start()
    {
        CreateButtons(_player.CurrentHandSize);
    }
    
    public void CreateButtons(int buttonNumber)
    {
        _handItems = new HandItem[buttonNumber];
        for (int i = 0; i < buttonNumber; i++)
        {
            GameObject obj = Instantiate(_abilityCardPrefab.gameObject, transform);
            obj.transform.SetSiblingIndex(i);
            _handItems[i] = new HandItem(obj.GetComponent<AbilityCardView>(), obj.GetComponent<Button>(), i, this);
            Debug.Log(_handItems[i].Obj.transform.position);
        }

        // animation object is a copy of the ability view
        _animObject = Instantiate(_abilityCardPrefab.gameObject, _mainCanvas.transform);
        _animObject.SetActive(false);
        RectTransform rect = _animObject.GetComponent<RectTransform>();
        _animStartSize = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        // TODO need to find a way to set the physical position under gridlayout, it doesn't seem to keep until they're activated
        if(!startBool)
        {
            for (int i = 0; i < _handItems.Length; i++)
            {
                _handItems[i].Obj.SetActive(false);
            }
            startBool = true;
        }

        gameObject.SetActive(true);

        if (_lastCount > deck.Count)
        {
            ClearList();
            for (int i = 0; i < deck.Count; i++)
            {
                _handItems[i].CardView.Display(deck.GetCard(i));
                _handItems[i].Obj.SetActive(true);
            }
        }
        else
        {
            // TODO disgusting
            for (int i = 0; i < deck.Count; i++)
            {
                _handItems[i].CardView.Display(deck.GetCard(i));
                _animObject.gameObject.GetComponent<AbilityCardView>()?.Display(deck.GetCard(i));
                Vector3 endPosition = _handItems[i].Obj.transform.position;
                GameObject currentObject = _handItems[i].Obj;
                _animObject.transform.position = _deckPosition.position;
                _animObject.SetActive(true);
                LeanTween.move(_animObject, endPosition, 0.25f).setOnComplete(() => { FinishAnimation(currentObject); });

                RectTransform rect = _animObject.GetComponent<RectTransform>();
                LeanTween.size(rect, new Vector3(_gridLayout.cellSize.x, _gridLayout.cellSize.y, 1), 0.25f).setOnComplete(
                    () => { rect.sizeDelta.Set(_animStartSize.x, _animStartSize.y); } );
            }
            if (_drawAudio)
                AudioHelper.PlayClip2D(_drawAudio, 0.5f);
        }
        _lastCount = deck.Count;
    }

    public void FinishAnimation(GameObject obj)
    {
        _animObject.SetActive(false);
        
        obj.SetActive(true);
    }

    public void HideDeck()
    {
        ClearList();
        gameObject.SetActive(false);
    }

    
    public void ClearList()
    {
        if(_handItems.Length > 0)
        {
            foreach (HandItem item in _handItems)
            {
                item.Obj.SetActive(false);
            }
        }
    }
    

    // TODO this is really bad but I had no idea where else to put this functionality - need a better organizational approach
    public void UseCard(int index)
    {
        _player.PlayAbilityCard(index);
    }
}

public class HandItem
{
    public AbilityCardView CardView { get; set; }
    public PlayerHandView HandView { get; set; }
    public GameObject Obj { get; set; }
    public Button ItemButton { get; set; }
    public int Index { get; set; } = 0;
    
    public HandItem(AbilityCardView cardView, Button button, int index, PlayerHandView handView)
    {
        CardView = cardView;
        ItemButton = button;
        Index = index;
        HandView = handView;
        Obj = cardView.gameObject;
        ItemButton.onClick.AddListener(() => { HandView.UseCard(Index); });
    }
}
