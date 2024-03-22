using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> bulletPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bulletPool = new Queue<GameObject>();
        CreateInitialPool();
    }

    private void CreateInitialPool()
    {
        for (int i = 0; i < poolSize; ++i)
        {
            CreateNewBulletInPool();
        }
    }

    private void CreateNewBulletInPool()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform);
        newBullet.SetActive(false);
        bulletPool.Enqueue(newBullet);
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            CreateNewBulletInPool();
        }

        GameObject gettedBullet = bulletPool.Dequeue();
        gettedBullet.transform.parent = null;
        gettedBullet.SetActive(true);
        return gettedBullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.parent = gameObject.transform;
        bulletPool.Enqueue(bullet);
    }
}
