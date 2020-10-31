using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // TODO built a more structured connection, maybe singleton?
    public static ITargetable CurrentTarget;
    public static PlayerController CurrentPlayer;
    public static EnemyController CurrentEnemy;

    [SerializeField] Creature _objectToTarget = null;

    private void Awake()
    {
        CurrentPlayer = FindObjectOfType<PlayerController>();
        CurrentEnemy = FindObjectOfType<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Alpha 1 is numeral 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ITargetable possibleTarget = _objectToTarget.GetComponent<ITargetable>();
            if(possibleTarget != null)
            {
                Debug.Log("New target acquired!");
                
                _objectToTarget.Target();
            }
        }
    }

    public static void SetCurrentTarget(GameObject obj)
    {
        Debug.Log("Set Current Target as: " + obj.name);
        ITargetable possibleTarget = obj.GetComponent<ITargetable>();
        if (possibleTarget != null)
        {
            CurrentTarget = possibleTarget;
            CurrentTarget.Target();
        }
    }
}
