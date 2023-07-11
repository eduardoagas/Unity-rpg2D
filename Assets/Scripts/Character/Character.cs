using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public GameObject grassB;
    private GameObject grassClone = null;
    CharacterAnimator animator;
    public float moveSpeed;
    public bool isMoving;

    public CharacterAnimator Animator { get => animator; set => animator = value; }

    private void Awake(){
        Animator = GetComponent<CharacterAnimator>();
    }

     public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {   
        Animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        Animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);
        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        grassTreat(targetPos, moveVec);
        /*if(isWalkable(targetPos))
                    StartCoroutine(Move(targetPos));*/


        if (!IsPathClear(targetPos))
            yield break;


        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);  
            yield return null;
        }

        transform.position = targetPos; //once you deal with Mathf.Epsilon
        isMoving = false;

        OnMoveOver?.Invoke();
        
    }

    public void HandleUpdate(){
        animator.IsMoving = isMoving;
    }

    private bool IsPathClear(Vector3 targetpos){
        var diff = targetpos - transform.position;
        var dir = diff.normalized; //o vetor normalizado possui a mesma direção porém com grandeza de 1

        if( Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, 
          diff.magnitude - 1, GameLayers.i.SolidObjectsLayer | GameLayers.i.PlayerLayer | GameLayers.i.InteractableLayer) == true){
            return false;
          }
        return true;
    }

    void grassTreat(Vector3 targetPos, Vector2 input){
        
        if(grassClone != null){
            Destroy(grassClone);
            grassB.SetActive(false);
            Debug.Log("destruí");
        }
        if(Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.GrassLayer) != null){
                    if(input.y != 1){
                        //grassB.SetActive(true);
                         grassClone = Instantiate(grassB);
                         grassClone.SetActive(true);
                         //grassClone.GetComponent<SpriteRenderer>().enabled = true;
                         grassClone.transform.position = targetPos-new Vector3(0, 0.3f, 0);
                     }            
        } 
    }

    public void LookTowards(Vector3 targetPos){
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if(xdiff == 0 || ydiff == 0){
            Animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            Animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        } else
            Debug.LogError("Error in LookTowards: You can't ask the character to look diagonally.");
    }

    private bool IsWalkable(Vector3 targetPos){
        return !(Physics2D.OverlapCircle(targetPos, 0.1f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer) != null);
    }
}
