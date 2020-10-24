using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class PlayerHandView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    private HandItem[] _viewArray = null;
    private GridLayoutGroup _gridLayout = null;
    private PlayerController _player = null;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
        _gridLayout = GetComponent<GridLayoutGroup>();
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        gameObject.SetActive(true);
        ClearArray();
        _viewArray = new HandItem[deck.Count];
        for (int i = 0; i < deck.Count; i++)
        {
            _viewArray[i] = new HandItem(Instantiate(_abilityCardPrefab.gameObject, transform), i);
            _viewArray[i].Obj.GetComponent<AbilityCardView>()?.Display(deck.GetCard(i));
            Debug.Log("Index = " + i);
            _viewArray[i].Obj.GetComponent<Button>()?.onClick.AddListener(() => UseCard(_viewArray[i].Index));
        }
    }

    public void HideDeck()
    {
        ClearArray();
        gameObject.SetActive(false);
    }

    private void ClearArray()
    {
        if(_viewArray != null)
        {
            foreach (HandItem item in _viewArray)
            {
                item.Obj.GetComponent<Button>()?.onClick.RemoveAllListeners();
                Destroy(item.Obj);
            }
        }
    }

    // TODO this is really bad but I had no idea where else to put this functionality
    private void UseCard(int index)
    {
        _player.PlayAbilityCard(index);
    }
}

public class HandItem
{
    public GameObject Obj { get; set; }
    public int Index { get; set; }

    public HandItem(GameObject gameObject, int index)
    {
        Obj = gameObject;
        Index = index;
    }
}
