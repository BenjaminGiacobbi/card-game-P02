using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoostCardView))]
public class EnemyBoostCard : MonoBehaviour
{
    [SerializeField] CardGameController _controller = null;
    [SerializeField] Text _countText = null;
    private BoostCardView _view = null;

    private void Awake()
    {
        _view = GetComponent<BoostCardView>();
    }

    private void OnEnable()
    {
        _controller.CurrentBoostDeck += ShowBoostCard;
    }

    private void OnDisable()
    {
        _controller.CurrentBoostDeck -= ShowBoostCard;
    }

    private void ShowBoostCard(Deck<BoostCard> deck)
    {
        if (!deck.IsEmpty)
        {
            _view.Display(deck.TopItem);
        } 
        else
            gameObject.SetActive(false);
        _countText.text = _controller.BoostDeck.Count.ToString();
    }
}
