using UnityEngine;
using UnityEngine.UI;

public class DiscardDeckView : MonoBehaviour, IDeckView<AbilityCard>
{
    [SerializeField] AbilityCardView _abilityCardPrefab = null;
    [SerializeField] Text _discardCountText = null;

    private void Start()
    {
        Button button = _abilityCardPrefab.GetComponent<Button>();
        button.interactable = false;
        button.transition = Selectable.Transition.None;
        _abilityCardPrefab.transform.SetSiblingIndex(0);
        _abilityCardPrefab.transform.localPosition = Vector3.zero;
        _abilityCardPrefab.gameObject.SetActive(false);
        _discardCountText.text = "0";
    }

    public void ShowDeck(Deck<AbilityCard> deck)
    {
        if (!deck.IsEmpty)
        {
            _abilityCardPrefab.GetComponent<AbilityCardView>()?.Display(deck.TopItem);
            _abilityCardPrefab.gameObject.SetActive(true);
        }
        else
        {
            _abilityCardPrefab.gameObject.SetActive(false);
        }
        _discardCountText.text = deck.Count.ToString();
    }

    public void HideDeck()
    {
        _abilityCardPrefab.gameObject.SetActive(false);
        _abilityCardPrefab.GetComponent<AbilityCardView>()?.EmptyDisplay();
    }
}
