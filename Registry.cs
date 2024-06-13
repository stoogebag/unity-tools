using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry<T> 
{
    public Dictionary<string, T> _objects = new Dictionary<string, T>();
    
    public void Register(string guid, T obj)
    {
        _objects.Add(guid, obj);
    }
    
    public void Unregister(string guid)
    {
        _objects.Remove(guid);
    }
    
    public T Get(string guid)
    {
        return _objects[guid];
    }
    
    public bool TryGet(string guid, out T obj)
    {
        return _objects.TryGetValue(guid, out obj);
    }
    
    public bool Contains(string guid)
    {
        return _objects.ContainsKey(guid);
    }
    
    public void Clear()
    {
        _objects.Clear();
    }
    
    public void ForEach(System.Action<T> action)
    {
        foreach (var obj in _objects.Values)
        {
            action(obj);
        }
    }
}
