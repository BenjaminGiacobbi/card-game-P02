using UnityEngine;

public abstract class Card
{
    public string Name { get; protected set; } = "...";
    public string Description { get; protected set; } = "...";

    public abstract void Play();
}
