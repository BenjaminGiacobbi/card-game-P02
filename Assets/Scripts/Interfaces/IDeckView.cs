using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDeckView<T> where T : Card
{
    void ShowDeck(Deck<T> deck);
    void HideDeck();
}
