using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPlayEffect", menuName = "CardData/PlayEffects/Spawn")]
public class SpawnPlayEffect : CardPlayEffect
{
    [SerializeField] GameObject _prefabToSpawn = null;

    public override void Activate(ITargetable target)
    {
        // test if target has monobehavior to exist in world
        MonoBehaviour worldObject = target as MonoBehaviour;
        if (worldObject != null)
        {
            Vector3 spawnLocation = worldObject.transform.position;
            GameObject newGameObject = Instantiate
                (_prefabToSpawn, spawnLocation, Quaternion.identity);
            Debug.Log("Spawn new Object: " + newGameObject.name);
        }
        else
        {
            Debug.Log("Target does not have a transform.");
        }
    }
}
