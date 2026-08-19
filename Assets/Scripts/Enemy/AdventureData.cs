using System.Collections.Generic;
using UnityEngine;

public class AdventurerData : MonoBehaviour
{
    // ランクの定義
    public enum Rank
    {
        Iron,
        Bronze,
        Silver,
        Gold,
        Platinum,
        Emerald,
        Diamond,
        Master,
        Grandmaster,
        Challenger
    }

    [System.Serializable]
    public struct RankStatus
    {
        public Rank rank;
        public string rankName;
        public int maxHealth;
        public float moveSpeed;
        public int attackPower;
        public float attackInterval;
        public GameObject prefab;
    }

    [Header("全ランクのステータスデータ（10種類）")]
    public List<RankStatus> rankStatuses = new List<RankStatus>()
    {
        new RankStatus { rank = Rank.Iron,        rankName = "アイアン",       maxHealth = 100,  moveSpeed = 3.5f, attackPower = 10,  attackInterval = 1.2f },
        new RankStatus { rank = Rank.Bronze,      rankName = "ブロンズ",       maxHealth = 150,  moveSpeed = 3.6f, attackPower = 15,  attackInterval = 1.1f },
        new RankStatus { rank = Rank.Silver,      rankName = "シルバー",       maxHealth = 220,  moveSpeed = 3.7f, attackPower = 22,  attackInterval = 1.0f },
        new RankStatus { rank = Rank.Gold,        rankName = "ゴールド",       maxHealth = 320,  moveSpeed = 3.8f, attackPower = 32,  attackInterval = 0.95f },
        new RankStatus { rank = Rank.Platinum,    rankName = "プラチナ",       maxHealth = 450,  moveSpeed = 3.9f, attackPower = 45,  attackInterval = 0.9f },
        new RankStatus { rank = Rank.Emerald,     rankName = "エメラルド",     maxHealth = 600,  moveSpeed = 4.0f, attackPower = 60,  attackInterval = 0.85f },
        new RankStatus { rank = Rank.Diamond,     rankName = "ダイヤモンド",   maxHealth = 800,  moveSpeed = 4.1f, attackPower = 80,  attackInterval = 0.8f },
        new RankStatus { rank = Rank.Master,      rankName = "マスター",       maxHealth = 1100, moveSpeed = 4.2f, attackPower = 110, attackInterval = 0.75f },
        new RankStatus { rank = Rank.Grandmaster, rankName = "グラマス",       maxHealth = 1500, moveSpeed = 4.3f, attackPower = 150, attackInterval = 0.7f },
        new RankStatus { rank = Rank.Challenger,  rankName = "チャレンジャー", maxHealth = 2000, moveSpeed = 4.5f, attackPower = 200, attackInterval = 0.6f }
    };

    /// <summary>
    /// 指定したランクのステータスを取得する
    /// </summary>
    public RankStatus GetStatus(Rank rank)
    {
        return rankStatuses.Find(s => s.rank == rank);
    }
}