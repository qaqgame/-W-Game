using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LockStepController.Instance.StartGame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LockStepController.Instance.ConfirmTurn(LockStepController.Instance.LockStepTurnID+1);
    }
}
