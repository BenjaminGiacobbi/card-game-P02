using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class PlayerHandView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    private GameObject[] _viewArray = null;
    private GridLayoutGroup _gridLayout = null;

    private void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        gameObject.SetActive(true);
        ClearArray();
        _viewArray = new GameObject[deck.Count];
        for (int i = 0; i < deck.Count; i++)
        {
            _viewArray[i] = Instantiate(_abilityCardPrefab.gameObject, transform);
            _viewArray[i].GetComponent<AbilityCardView>()?.Display(deck.GetCard(i));
        }
    }

    private void ClearArray()
    {
        if(_viewArray != null)
        {
            foreach (GameObject obj in _viewArray)
            {
                Destroy(obj);
            }
        }
    }
}
