using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Pool> poolConfig;

    private void Awake()
    {
        Instance = this;
        SetUp();
    }

    private void SetUp()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolConfig = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolConfig.Add(pool.tag, pool);
        }
    }

    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];

        if (objectPool.Count > 0 && !objectPool.Peek().activeSelf)
        {
            GameObject obj = objectPool.Dequeue();
            obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            obj.SetActive(true);
            objectPool.Enqueue(obj);
            return obj;
        }
        else
        {
            Pool pool = poolConfig[tag];
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            obj.SetActive(true);

            objectPool.Enqueue(obj);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
}
