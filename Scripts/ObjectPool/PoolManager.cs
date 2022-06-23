using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    public List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private void OnEnable()
    {
        EventHandler.ParticaleEffectEvent += OnParticaleEffectEvent; 
    }
    private void Disable()
    {
        EventHandler.ParticaleEffectEvent -= OnParticaleEffectEvent; 
    }

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                ()=>Instantiate(item,parent),
                e=>{e.SetActive(true);},
                e => {e.SetActive(false);},
                e => {Destroy(e);}

            );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticaleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        ObjectPool<GameObject> objPool = effectType switch
        {

            ParticaleEffectType.LeaveFalling1 => poolEffectList[0],
            ParticaleEffectType.LeaveFalling2 => poolEffectList[1],
            ParticaleEffectType.Rock => poolEffectList[2],
            ParticaleEffectType.ReapableScenery => poolEffectList[3],
            _ => null,
        };
        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }
}