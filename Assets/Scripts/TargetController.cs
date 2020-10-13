using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // TODO built a more structured connection, maybe singleton?
    public static ITargetable CurrentTarget;

    [SerializeField] Creature _objectToTarget = null;

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
                CurrentTarget = possibleTarget;
                _objectToTarget.Target();
            }
        }
    }
}
