﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPlayEffect", menuName = "CardData/PlayEffects/Spawn")]
public class SpawnPlayEffect : CardPlayEffect
{
    [SerializeField] Creature _creatureToSpawn = null;
    public Creature Creature => _creatureToSpawn;

    public override void Activate(ITargetable target)
    {
        // test if target has monobehavior to exist in world
        BoardSpace space = target as BoardSpace;
        if (space != null)
        {
            GameObject newCreature = ObjectPooler.Instance.SpawnObject(
                Creature.Name, space.transform, space.SpawnLocation.position, space.SpawnLocation.transform.rotation);
            Creature creatureScript = newCreature.GetComponent<Creature>();
            creatureScript.OnSpawn();
            space.Creature = creatureScript;
            space.SpawnFeedback();
            if (space.Creature == null)
            {
                Debug.Log("Creature Missing");
            }
        }
    }
}
