using System;
using System.Collections;
using System.Collections.Generic;
using stoogebag.Extensions;
using stoogebag.Utils;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingManager : Singleton<PoolingManager>
{
    private Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();
    // Start is called before the first frame update

    private ObjectPool<GameObject> RegisterPool(string key, GameObject prefab)
    {
        var pool = new ObjectPool<GameObject>(()=>Instantiate(prefab), 
            emission => emission.SetActive(true), 
            emission => emission.SetActive(false), 
            emission => Destroy(emission));
        
        _pools.Add(key, pool);

        return pool;
    }
    
    public ObjectPool<GameObject> GetOrCreatePool(string key, GameObject prefab = null)
    {
        return _pools.TryGetOrAdd(key, () =>
        {
            if (prefab == null) throw new Exception("Prefab cannot be null!");
            return RegisterPool(key, prefab);
        });
    }




}
