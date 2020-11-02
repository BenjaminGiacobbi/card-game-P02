using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck<T> where T : Card // called a constraint
{
    List<T> _cards = new List<T>();

    public event Action Emptied = delegate { };
    public event Action<T> CardAdded = delegate { };
    public event Action<T> CardRemoved = delegate { };

    public int Count => _cards.Count;
    public T TopItem => _cards[_cards.Count - 1];
    public T BottomItem => _cards[0];
    public bool IsEmpty => _cards.Count == 0;
    public int LastIndex
    {
        get
        {
            if(_cards.Count == 0)
            {
                return 0;
            }
            else
            {
                return _cards.Count - 1;
            }
        }
    }

    private int GetIndexFromPosition(DeckPosition position)
    {
        int newPositionIndex = 0;
        // Index is zero on an empty deck, though this will have to be interpreted
        if (_cards.Count == 0)
            newPositionIndex = 0;

        // Index is last in deck "on the top"
        if (position == DeckPosition.Top)
        {
            newPositionIndex = LastIndex;
        }
        // randomizes index if drawing from deck "body"
        else if (position == DeckPosition.Middle)
        {
            newPositionIndex = UnityEngine.Random.Range(0, LastIndex);
        }
        // Index is 0 "on the bottom"
        else if (position == DeckPosition.Bottom)
        {
            newPositionIndex = 0;
        }

        return newPositionIndex;
    }

    // Inserts a single card into the list based on index position)
    public void Add(T card, DeckPosition position = DeckPosition.Top)
    {
        // Bodyguards on bad call
        if (card == null) { return; }

        int targetIndex = GetIndexFromPosition(position);
        if (targetIndex == LastIndex)
        {
            _cards.Add(card);
        }
        else
        {
            _cards.Insert(targetIndex, card);
        }

        CardAdded?.Invoke(card);
    }

    // adds cards to the deck from a given list
    public void Add(List<T> cards, DeckPosition position = DeckPosition.Top)
    {
        int itemCount = cards.Count;
        for (int i = 0; i < itemCount; i++)
        {
            Add(cards[i], position);
        }
    }

    // retrieves an item while removing it from the deck
    public T Draw(DeckPosition position = DeckPosition.Top)
    {
        if (IsEmpty)
        {
            Debug.LogWarning("Deck: Can't DRAW new item from empty deck!");
            return default;
        }

        int targetIndex = GetIndexFromPosition(position);
        if(IsIndexWithinListRange(targetIndex))
        {
            T cardToRemove = _cards[targetIndex];
            Remove(targetIndex);

            return cardToRemove;
        }
        else
        {
            return default;
        }
    }

    // removes card without retrieving (though signals the removed card)
    public void Remove(int index)
    {
        if (IsEmpty)
        {
            Debug.LogWarning("Deck: Can't REMOVE from an empty deck!");
            return;
        }
        else if (!IsIndexWithinListRange(index))
        {
            Debug.LogWarning("Deck: Call is out of range. Cannot REMOVE item.");
            return;
        }

        T removedItem = _cards[index];
        _cards.RemoveAt(index);

        CardRemoved?.Invoke(removedItem);

        if (_cards.Count == 0)
            Emptied?.Invoke();
    }

    public void Empty()
    {
        int deckCount = Count;
        for(int i = 0; i < deckCount; i++)
        {
            Remove(0);
        }
    }

    // retrieves card without removing, though no signal
    public T GetCard(int index)
    {
        if (_cards[index] != null && IsIndexWithinListRange(index))
        {
            return _cards[index];
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Randomly shuffles cards, from the bottom up
    /// </summary>
    public void Shuffle()
    {
        // start at the top and randomly swap cards on the way down
        for(int currentIndex = Count - 1; currentIndex > 0; --currentIndex)
        {
            // chooses a random card beyond currentIndex
            int randomIndex = UnityEngine.Random.Range(0, currentIndex + 1);
            T randomCard = _cards[randomIndex];

            // swaps the random card with the current index
            _cards[randomIndex] = _cards[currentIndex];
            _cards[currentIndex] = randomCard;
        }
    }

    // validate a card call if index is within list size
    bool IsIndexWithinListRange(int index)
    {
        if (index >= 0 && index <= _cards.Count - 1)
            return true;

        Debug.LogWarning("Deck: Index out of range;" + " index: " + index);
        return false;
    }
}
