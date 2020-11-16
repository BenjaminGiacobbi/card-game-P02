using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : GenericSingleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> Pools;
    public Dictionary<string, Queue<GameObject>> _poolDictionary;

    // create pools
    private void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnObject(string tag, Transform parent, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " does not exist.");
            return null;
        }

        GameObject spawnObject = _poolDictionary[tag].Dequeue();
        spawnObject.SetActive(true);
        spawnObject.transform.SetParent(parent);
        spawnObject.transform.position = position;
        spawnObject.transform.rotation = rotation;

        return spawnObject;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " does not exist.");
            return;
        }
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _poolDictionary[tag].Enqueue(obj);
    }
}
