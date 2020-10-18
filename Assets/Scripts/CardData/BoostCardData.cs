using UnityEngine;

[CreateAssetMenu(fileName = "NewBoostCard", menuName = "CardData/BoostCard")]
public class BoostCardData : ScriptableObject
{
    [SerializeField] string _name = "...";
    public string Name => _name;

    [SerializeField] int _uses = 2;
    public int Uses => _uses;

    [SerializeField] Sprite _graphic = null;
    public Sprite Graphic => _graphic;

    [SerializeField] CardPlayEffect _playEffect = null;
    public CardPlayEffect PlayEffect => _playEffect;
}
