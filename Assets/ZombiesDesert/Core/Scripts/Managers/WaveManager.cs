using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Collider spawnZone;

    [SerializeField] private EnemyPrototypeSelector prototypeSelector;

    private List<IEnemy> enemies = new List<IEnemy>();

    public void CreateArmy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
        }
    }

    private void Start()
    {
        CreateArmy(5);
        StartCoroutine(CommandArmyAttack());
    }

    IEnumerator CommandArmyAttack()
    {
        yield return new WaitForEndOfFrame();

        foreach (IEnemy enemy in enemies)
        {
            enemy.EnterBattleMode();
        }
    }

    private void SpawnEnemy()
    {
        //Instantiate(zombiePrefab, GetRandomPointInSpawnZone(), Quaternion.identity);

        IEnemy enemy = prototypeSelector.CreateZombie(UnitType.Zombie, GetRandomPointInSpawnZone());
        enemies.Add(enemy);
    }

    private Vector3 GetRandomPointInSpawnZone()
    {
        return new Vector3(
            Random.Range(spawnZone.bounds.min.x, spawnZone.bounds.max.x),
            Random.Range(spawnZone.bounds.min.y, spawnZone.bounds.max.y),
            Random.Range(spawnZone.bounds.min.z, spawnZone.bounds.max.z));
    }
}
