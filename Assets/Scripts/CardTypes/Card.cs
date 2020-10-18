using UnityEngine;

public abstract class Card
{
    public string Name { get; protected set; } = "...";

    public abstract void Play();
}
