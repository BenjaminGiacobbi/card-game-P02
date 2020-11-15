using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityCard", menuName = "CardData/AbilityCard")]
public class AbilityCardData : ScriptableObject
{
    [SerializeField] string _name = "...";
    public string Name => _name;

    [SerializeField] string _description = "...";
    public string Description => _description;

    [SerializeField] int _cost = 1;
    public int Cost => _cost;

    [SerializeField] Sprite _graphic = null;
    public Sprite Graphic => _graphic;

    [SerializeField] CardPlayEffect _playEffect = null;
    public CardPlayEffect PlayEffect => _playEffect;

    [SerializeField] AbilityType _type;
    public AbilityType Type => _type;
}

public enum AbilityType
{
    Spawn,
    Damage
}
