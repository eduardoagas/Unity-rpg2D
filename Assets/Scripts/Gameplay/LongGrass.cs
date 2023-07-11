using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        if(UnityEngine.Random.Range(1,101) <= 10){
                //animator.SetBool("isMoving", false);
                GameController.Instance.StartBattle();
            }

       /* if(player.gameObject.GetComponent<Character>().isMoving){
               player.gameObject.GetComponentInChildren<GrassB>().gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }else{
            player.gameObject.GetComponentInChildren<GrassB>().gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }*/            
    }
}

