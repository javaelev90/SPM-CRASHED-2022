using UnityEngine;
using BehaviorTree;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int lowThreshHold;
    [SerializeField] private float chasingRange;
    [SerializeField] private float explodingRange;
    [SerializeField] private Transform playerTransform;

    public int Health
    { get { return health; } }

    public int LowThreshHold 
    { get { return lowThreshHold; } }
}