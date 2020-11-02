using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPlayEffect", menuName = "CardData/PlayEffects/Spawn")]
public class SpawnPlayEffect : CardPlayEffect
{
    [SerializeField] Creature _creatureToSpawn = null;
    private GameObject _spawnedObject = null;

    public override void Activate(ITargetable target)
    {
        // test if target has monobehavior to exist in world
        BoardSpace space = target as BoardSpace;
        if (space != null)
        {
            GameObject newCreature = Instantiate
                (_creatureToSpawn.gameObject, space.SpawnLocation.position, Quaternion.identity);
            newCreature.transform.parent = space.transform;
            newCreature.transform.localRotation = Quaternion.Euler(newCreature.transform.localRotation.x, 0, newCreature.transform.localRotation.z);
            space.Creature = newCreature.GetComponent<Creature>();
            Debug.Log("Spawn new Object: " + space.name);
        }
        else
        {
            Debug.Log("Target is not a board space.");
        }
    }

    /*
    public Creature GetCreature()
    {
        Creature instantiatedCreature = _spawnedObject.GetComponent<Creature>();
        if(instantiatedCreature != null)
            return _spawnedObject.GetComponent<Creature>();
        else
            return null;
    }
    */
}
