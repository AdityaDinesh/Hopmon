using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PoolController : MonoBehaviour
{
    public static PoolController Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public AssetReferenceGameObject addressableRef;
        public int size;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, List<AsyncOperationHandle<GameObject>>> instanceHandles;

    public bool IsReady { get; private set; } = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        instanceHandles = new Dictionary<string, List<AsyncOperationHandle<GameObject>>>(); 

        StartCoroutine(InitializePool());
    }

    public IEnumerator InitializePool()
    {
        IsReady = false;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

            for (int i = 0; i < pool.size; i++)
            {
                var instHandle = pool.addressableRef.InstantiateAsync();
                yield return instHandle;

                if (instHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"[Pool] Failed to instantiate for tag: {pool.tag}");
                    continue;
                }

                GameObject obj = instHandle.Result;

                PoolObject poolObj = obj.GetComponent<PoolObject>();
                if (poolObj != null)
                    poolObj.poolTag = pool.tag;

                obj.SetActive(false);
                objectPool.Enqueue(obj);
                handles.Add(instHandle); // track the INSTANCE handle, not load handle

                //if (i % 10 == 0)
                //    yield return null;
            }

            poolDictionary[pool.tag] = objectPool;
            instanceHandles[pool.tag] = handles;
            yield return null;
        }

        IsReady = true;
        Debug.Log("[Pool] All pools initialized.");
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!IsReady)
        {
            Debug.LogWarning("[Pool] Pool not ready yet!");
            return null;
        }

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"[Pool] Tag '{tag}' doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"[Pool] Tag '{tag}' pool is empty!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.SetActive(true);

        PoolObject poolObject = objectToSpawn.GetComponent<PoolObject>();
        if (poolObject != null)
            poolObject.Setup();

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    private void OnDestroy()
    {
        foreach (var kvp in instanceHandles)
        {
            foreach (var handle in kvp.Value)
            {
                // Only release valid, succeeded handles
                if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                    Addressables.Release(handle);
            }
        }

        instanceHandles.Clear();
        poolDictionary.Clear();
    }
}