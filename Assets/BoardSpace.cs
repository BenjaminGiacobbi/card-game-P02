using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour
{
    [SerializeField] Text _cardText = null;
    public AbilityCard CardData { get; private set; }

    private void Start()
    {
        _cardText.text = "";
    }

    public void SetCard(AbilityCard card)
    {
        CardData = card;
        _cardText.text = CardData.Name;
    }

    // methods for handling visuals on board
}
