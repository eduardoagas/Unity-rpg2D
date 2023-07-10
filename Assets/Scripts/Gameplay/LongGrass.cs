using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        if(UnityEngine.Random.Range(1,101) <= 10){
                //animator.SetBool("isMoving", false);
                
                GameController.Instance.StartBattle();
            }
    }
}

