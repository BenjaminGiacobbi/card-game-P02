using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPlayEffect", menuName = "CardData/PlayEffects/Spawn")]
public class SpawnPlayEffect : CardPlayEffect
{
    [SerializeField] ParticleSystem _spawnParticles = null;
    [SerializeField] AudioClip _spawnAudio = null;
    [SerializeField] Creature _creatureToSpawn = null;
    public Creature Creature => _creatureToSpawn;
    // private GameObject _spawnedObject = null;

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
            if (_spawnParticles)
            {
                GameObject obj = Instantiate(_spawnParticles.gameObject, space.SpawnLocation.position, _spawnParticles.gameObject.transform.rotation);
                obj.GetComponent<ParticleSystem>().Play();
            }
            if (_spawnAudio)
                AudioHelper.PlayClip2D(_spawnAudio, 0.5f);
            if (space.Creature == null)
            {
                Debug.Log("Creature Missing");
            }
        }
        else
        {
            Debug.Log("Target is not a board space.");
        }
    }
}
