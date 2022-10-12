using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PrefabPool : MonoBehaviour
{

    [SerializeField] private GameObject prefabToBePooled;
    [SerializeField] private int poolStartAmount;
    [SerializeField] private List<GameObject> objectPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        FillPool();
    }

    private void FillPool()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int i = 0; i < poolStartAmount; i++)
        {
            var obj = Instantiate(prefabToBePooled);
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            objectPool.Add(obj);
        }
        stopWatch.Stop();
        UnityEngine.Debug.Log($"The {prefabToBePooled.name} pool was filled in {stopWatch.ElapsedMilliseconds}ms.");
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        var newObj = Instantiate(prefabToBePooled);
        newObj.SetActive(false);
        newObj.transform.parent = this.transform;
        objectPool.Add(newObj);

        return objectPool[objectPool.IndexOf(newObj)];
    }
}
