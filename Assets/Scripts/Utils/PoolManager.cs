using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;

        public int initialSize = 10;


    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<GameObject, string> instanceToPoolName = new Dictionary<GameObject, string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                instanceToPoolName[obj] = pool.prefab.name;
            }

            poolDictionary.Add(pool.prefab.name, objectPool);
        }
    }

    public GameObject Get(GameObject poolObject, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolObject.name))
        {
            Debug.LogError($"No existe un pool con nombre: {poolObject.name}");
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[poolObject.name];
        GameObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        else
        {
            // Buscar prefab base en la lista
            GameObject prefab = pools.Find(p => p.prefab.name == poolObject.name)?.prefab;
            if (prefab == null)
            {
                Debug.LogError($"No se encuentra prefab para pool: {poolObject.name}");
                return null;
            }

            obj = Instantiate(prefab);
            instanceToPoolName[obj] = poolObject.name;
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    public void Return(GameObject obj, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(ReturnAfterDelay(obj, delay));
        }
        else
        {
            ReturnImmediately(obj);
        }
    }
    private IEnumerator ReturnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnImmediately(obj);
    }
    private void ReturnImmediately(GameObject obj)
    {
        if (!instanceToPoolName.ContainsKey(obj))
        {
            Debug.LogWarning("Este objeto no pertenece a ningún pool.");
            Destroy(obj);
            return;
        }
        Debug.Log($"Devolviendo objeto al pool: {obj.name}");
        string poolName = instanceToPoolName[obj];
        obj.SetActive(false);
        poolDictionary[poolName].Enqueue(obj);
    }
}
