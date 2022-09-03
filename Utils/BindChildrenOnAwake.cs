using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using stoogebag;

public class BindChildrenOnAwake : MonoBehaviour
{
    
    private void Awake()
    {
        var components = GetComponents<MonoBehaviour>();

        for (var i = 0; i < components.Length; i++)
        {
            var con = components[i];
            foreach (var info in con.GetAllFieldsWithAttribute(typeof(InitialiseOnAwakeAttribute), true))
            {
                Type t = info.FieldType;
                MethodInfo method = typeof(GameObjectExtensions).GetMethod("GetChild");
                MethodInfo generic = method.MakeGenericMethod(t);
                var child = generic.Invoke(this,  new object[]{con, info.Name});
                info.SetValue(con,child);
            }
        }
    }

    private void Start()
    {
        var components = GetComponents<MonoBehaviour>();

        for (var i = 0; i < components.Length; i++)
        {
            var con = components[i];
            foreach (var info in con.GetAllFieldsWithAttribute(typeof(InitialiseOnStartAttribute), true))
            {
                Type t = info.FieldType;
                MethodInfo method = typeof(GameObjectExtensions).GetMethod("GetChild");
                MethodInfo generic = method.MakeGenericMethod(t);
                var child = generic.Invoke(this,  new object[]{con, info.Name});
                info.SetValue(con,child);
            }
        }
    }
}

public class InitialiseOnAwakeAttribute : Attribute
{
    public string Name;
    public InitialiseOnAwakeAttribute() {
    }
    public InitialiseOnAwakeAttribute(string name)
    {
        Name = name;
    }
}

public class InitialiseOnStartAttribute : Attribute
{
    public string Name;
    public InitialiseOnStartAttribute() {
    }
    public InitialiseOnStartAttribute(string name)
    {
        Name = name;
    }
}
