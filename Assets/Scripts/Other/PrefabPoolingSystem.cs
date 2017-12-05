using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PoolablePrefabSettings
{
    public GameObject Prefab;
    public int numberToSpawn;
}

public interface IPoolableComponent
{
    void Spawned();
    void Despawned();
}

public struct PoolablePrefabData
{
    public GameObject go;
    public IPoolableComponent[] poolableComponents;
}

public class PrefabPool
{
    Dictionary<GameObject, PoolablePrefabData> _activeList = new Dictionary<GameObject, PoolablePrefabData>();
    Queue<PoolablePrefabData> _inactiveList = new Queue<PoolablePrefabData>();

    public GameObject Spawn (GameObject prefab, Vector3 position, Quaternion rotation)
    {
        PoolablePrefabData data;
        if (_inactiveList.Count > 0)
        {
            data = _inactiveList.Dequeue();
        }
        else
        {
            GameObject newGo = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            data = new PoolablePrefabData();
            data.go = newGo;
            data.poolableComponents = newGo.GetComponents<IPoolableComponent>();
        }

        data.go.SetActive(true);
        data.go.transform.position = position;
        data.go.transform.rotation = rotation;
        for (int i = 0; i < data.poolableComponents.Length; i++)
        {
            data.poolableComponents[i].Spawned();
        }

        _activeList.Add(data.go, data);

        return data.go;
    }

    public bool Despawn(GameObject objToDespawn)
    {
        if (!_activeList.ContainsKey(objToDespawn))
        {
            Debug.LogError("This Object is not managed by this object pool!");
            return false;
        }

        PoolablePrefabData data = _activeList[objToDespawn];

        for (int i = 0; i < data.poolableComponents.Length; i++)
        {
            data.poolableComponents[i].Despawned();
        }
        data.go.SetActive(false);

        _activeList.Remove(objToDespawn);
        _inactiveList.Enqueue(data);
        return true;
    }
}

public static class PrefabPoolingSystem 
{
    static Dictionary<GameObject, PrefabPool> _prefabToPool = new Dictionary<GameObject, PrefabPool>();
    static Dictionary<GameObject, PrefabPool> _goToPool = new Dictionary<GameObject, PrefabPool>();

    public static GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_prefabToPool.ContainsKey(prefab))
        {
            _prefabToPool.Add(prefab, new PrefabPool());
        }
        PrefabPool pool = _prefabToPool[prefab];
        GameObject go = pool.Spawn(prefab, position, rotation);
        _goToPool.Add(go, pool);
        return go;
    }

    public static bool Despawn(GameObject obj)
    {
        if (!_goToPool.ContainsKey(obj))
        {
            Debug.LogError(string.Format("Object {0} not managed by pool system!", obj.name));
            return false;
        }

        PrefabPool pool = _goToPool[obj];
        if (pool.Despawn(obj))
        {
            _goToPool.Remove(obj);
            return true;
        }
        return false;
    }

    public static void Prespawn(GameObject prefab, int numToSpawn)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();

        for (int i = 0; i < numToSpawn; i++)
        {
            spawnedObjects.Add(Spawn(prefab));
        }
        for (int i = 0; i < numToSpawn; i++)
        {
            Despawn(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
    }

    public static void Reset()
    {
        _prefabToPool.Clear();
        _goToPool.Clear();
    }
}
