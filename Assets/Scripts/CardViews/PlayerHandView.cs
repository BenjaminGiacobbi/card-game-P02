using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridLayoutGroup))]
public class PlayerHandView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    private HandItem[] _handItems = null;
    private GridLayoutGroup _gridLayout = null;
    private PlayerController _player = null;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
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
            obj.SetActive(false);
        }
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        gameObject.SetActive(true);
        ClearList();
        for (int i = 0; i < deck.Count; i++)
        {
            _handItems[i].CardView.Display(deck.GetCard(i));
            _handItems[i].Obj.SetActive(true); 
        }
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
        Debug.Log("Index: " + index);
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
