/**
 *    Copyright (C) 2017 tongtunggiang.com
 *
 *    This program is free software: you can redistribute it and/or  modify
 *    it under the terms of the GNU Affero General Public License, version 3,
 *    as published by the Free Software Foundation.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU Affero General Public License for more details.
 *
 *    You should have received a copy of the GNU Affero General Public License
 *    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pool : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    int initialPoolsize = 10;

    [SerializeField] private bool extendable = true;
    [SerializeField] private bool reusable = true;

    Stack<GameObject> pooledInstances;
    List<GameObject> aliveInstances;

    public GameObject Prefab { get { return prefab; } }

    void Awake()
    {
        pooledInstances = new Stack<GameObject>();
        for (int i = 0; i < initialPoolsize; i++)
        {
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.zero;
            instance.SetActive(false);

            pooledInstances.Push(instance);
        }

        aliveInstances = new List<GameObject>();
    }

    /// <summary>
    /// Bring a new game object to life by taking a dead one from
    /// the waiting pool. If all objects in the pool are alive, 
    /// create a new one and add it to the pool.
    /// </summary>
    public GameObject Spawn(Vector3 position, 
        Quaternion rotation, 
        Vector3 scale, 
        Transform parent = null,
        bool useLocalPosition = false,
        bool useLocalRotation = false)
    {
        if (pooledInstances.Count <= 0) // Every game object has been spawned!
        {
            if (!extendable)
                return null;

            GameObject newlyInstantiatedObject = Instantiate(prefab);
            newlyInstantiatedObject.transform.SetParent(parent);
            newlyInstantiatedObject.SetActive(false);
            if (useLocalPosition)
                newlyInstantiatedObject.transform.localPosition = position;
            else
                newlyInstantiatedObject.transform.position = position;

            if (useLocalRotation)
                newlyInstantiatedObject.transform.localRotation = rotation;
            else
                newlyInstantiatedObject.transform.rotation = rotation;

            newlyInstantiatedObject.transform.localScale = scale;
            newlyInstantiatedObject.SetActive(true);
            aliveInstances.Add(newlyInstantiatedObject);
            return newlyInstantiatedObject;
        }

        GameObject obj = pooledInstances.Pop();

        obj.transform.SetParent(parent);

        if (useLocalPosition)
            obj.transform.localPosition = position;
        else
            obj.transform.position = position;

        if (useLocalRotation)
            obj.transform.localRotation = rotation;
        else
            obj.transform.rotation = rotation;
        obj.transform.localScale = scale;

        obj.SetActive(true);

        aliveInstances.Add(obj);

        return obj;
    }

    /// <summary>
    /// Deactivate an object and add it back to the pool, given that it's
    /// in alive objects array.
    /// </summary>
    /// <param name="obj"></param>
    public void Kill(GameObject obj)
    {
        int index = aliveInstances.FindIndex(o => obj == o);
        if (index == -1)
        {
            Destroy(obj);
            return;
        }

        if (extendable && !reusable && pooledInstances.Count == initialPoolsize)
        {
            if (index != -1)
                aliveInstances.RemoveAt(index);
            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;

        aliveInstances.RemoveAt(index);
        pooledInstances.Push(obj);
    }

    public bool IsResponsibleForObject(GameObject obj)
    {
        int index = aliveInstances.FindIndex(o => ReferenceEquals(obj, o));
        if (index == -1)
        {
            return false;
        }

        return true;
    }

    public GameObject RetrieveObject()
    {
        if (aliveInstances.Count > 0)
            return aliveInstances.Last();
        
        return null;
    }
}
