using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    // モンスターの種類
    public enum MonsterType
    {
        Spider,
        Goblin,
        Gargoyle,
        Skeleton,
        Daemon,
        Golem
    }

    [System.Serializable]
    public struct MonsterStatus
    {
        public MonsterType monsterType;
        public string monsterName;

        public int maxHealth;
        public float moveSpeed;
        public int attackPower;

        public float attackRange;
        public float detectionRange;
        public float attackInterval;

        public int buildCost;

        public GameObject prefab;
    }

    [Header("全モンスターのステータスデータ（6種類）")]
    public List<MonsterStatus> monsterStatuses = new List<MonsterStatus>()
    {
        new MonsterStatus
        {
            monsterType = MonsterType.Spider,
            monsterName = "スパイダー",
            maxHealth = 80,
            moveSpeed = 3.5f,
            attackPower = 15,
            attackRange = 1.5f,
            detectionRange = 6f,
            attackInterval = 0.7f,
            buildCost = 10
        },

        new MonsterStatus
        {
            monsterType = MonsterType.Goblin,
            monsterName = "ゴブリン",
            maxHealth = 100,
            moveSpeed = 3.0f,
            attackPower = 20,
            attackRange = 1.5f,
            detectionRange = 5f,
            attackInterval = 1.0f,
            buildCost = 15
        },

        new MonsterStatus
        {
            monsterType = MonsterType.Gargoyle,
            monsterName = "ガーゴイル",
            maxHealth = 150,
            moveSpeed = 2.5f,
            attackPower = 25,
            attackRange = 1.7f,
            detectionRange = 5f,
            attackInterval = 1.2f,
            buildCost = 25
        },

        new MonsterStatus
        {
            monsterType = MonsterType.Skeleton,
            monsterName = "スケルトン",
            maxHealth = 120,
            moveSpeed = 2.8f,
            attackPower = 30,
            attackRange = 1.5f,
            detectionRange = 6f,
            attackInterval = 1.0f,
            buildCost = 20
        },

        new MonsterStatus
        {
            monsterType = MonsterType.Daemon,
            monsterName = "デーモン",
            maxHealth = 180,
            moveSpeed = 2.2f,
            attackPower = 40,
            attackRange = 2.0f,
            detectionRange = 7f,
            attackInterval = 1.5f,
            buildCost = 35
        },

        new MonsterStatus
        {
            monsterType = MonsterType.Golem,
            monsterName = "ゴーレム",
            maxHealth = 300,
            moveSpeed = 1.5f,
            attackPower = 60,
            attackRange = 2.0f,
            detectionRange = 5f,
            attackInterval = 2.0f,
            buildCost = 50
        }
    };

    /// <summary>
    /// 指定したモンスターのステータスを取得する
    /// </summary>
    public MonsterStatus GetStatus(MonsterType monsterType)
    {
        return monsterStatuses.Find(
            s => s.monsterType == monsterType
        );
    }
}