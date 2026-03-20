using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float baseHP = 100f;
    public float baseDamage = 10f;
    public float attackCooldown = 1.5f;
}