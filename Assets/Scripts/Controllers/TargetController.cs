using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // TODO built a more structured connection, maybe singleton?
    public static ITargetable CurrentTarget { get; set; }
    public static PlayerController CurrentPlayer { get; private set; }
    public static EnemyController CurrentEnemy { get; private set; }

    [SerializeField] PlayerController _player = null;
    [SerializeField] EnemyController _enemy = null;

    private void Start()
    {
        CurrentPlayer = _player;
        CurrentEnemy = _enemy;
    }

    public static void SetTargetToPlayer()
    {
        CurrentTarget = CurrentPlayer.GetComponent<ITargetable>();
    }


    public static void SetTargetToEnemy()
    {
        CurrentTarget = CurrentEnemy.GetComponent<ITargetable>();
    }
}
