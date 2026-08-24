using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
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

    [Header("全モンスターのステータスデータ")]
    // ★初期データの new List を削除し、インスペクターからの入力のみにする
    public List<MonsterStatus> monsterStatuses = new List<MonsterStatus>();

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