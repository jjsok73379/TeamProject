using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    Dictionary<string, Queue<GameObject>> myPool = new Dictionary<string, Queue<GameObject>>();
    
    public T GetObject<T>(GameObject org, Vector3 pos, Quaternion rot)
    {
        string Name = typeof(T).ToString();
        if (myPool.ContainsKey(Name))
        {
            if (myPool[Name].Count > 0)
            {
                GameObject obj = myPool[Name].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                return obj.GetComponent<T>();
            }
        }
        else
        {
            myPool[Name] = new Queue<GameObject>();
        }
        return Instantiate(org, pos, rot).GetComponent<T>();
    }

    public void ReleaseObject<T>(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        myPool[typeof(T).ToString()].Enqueue(obj);
    }
}