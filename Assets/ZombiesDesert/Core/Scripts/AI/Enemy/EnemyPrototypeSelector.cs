using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrototypeSelector : MonoBehaviour
{
    [Serializable]
    private struct UnitPrototype
    {
        public UnitType unitType;
        public GameObject unitPrefab;
    }

    [SerializeField] private UnitPrototype[] unitPrototypes;

    public IEnemy CreateZombie(UnitType unitType, Vector3 position)
    {
        foreach (UnitPrototype prototype in unitPrototypes)
        {
            if (prototype.unitType == unitType)
            {
                GameObject prefab = prototype.unitPrefab;
                return Instantiate(prefab, position, Quaternion.identity).GetComponent<IEnemy>();
            }
        }

        throw new KeyNotFoundException($"prototype for unit of type {unitType} not found");
    }

}
