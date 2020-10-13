using UnityEngine;

[CreateAssetMenu(fileName = "NewDamagePlayEffect", menuName = "CardData/PlayEffects/Damage")]
public class DamagePlayEffect : CardPlayEffect
{
    [SerializeField] int _damageAmount = 1;

    public override void Activate(ITargetable target)
    {
        // test for damageable interace, then apply damage
        IDamageable objectToDamage = target as IDamageable; // method to retrieve interface without GetComponent
        if (objectToDamage != null)
        {
            objectToDamage.TakeDamage(_damageAmount);
            Debug.Log("Add damage to target.");
        }
        else
        {
            Debug.Log("Target is invulnerable.");
        }
    }
}
