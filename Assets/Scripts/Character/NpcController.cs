using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
 
    Character character;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;    
    private void Awake() {
        character = GetComponent<Character>();    
    }
    public void Interact(Transform initiator){
        if(state == NPCState.Idle){
            state = NPCState.Dialogue;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogueManager.Instance.ShowDialog(dialogue, ()=>{
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
        }    
    }   

    private void Update() {
        if(DialogueManager.Instance.isShowing) return;// comment this line to permit npcs to move around during dialogue
        if(state == NPCState.Idle){
            idleTimer += Time.deltaTime;
            //if(idleTimer > 2f){//every 2 seconds it wll leave idle
            if(idleTimer > timeBetweenPattern){
                idleTimer = 0f;
                if(movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }


    character.HandleUpdate();
    }

    
    IEnumerator Walk(){
        
        var oldPos = transform.position;
        state = NPCState.Walking;
        yield return character.Move(movementPattern[currentPattern]);
        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        state = NPCState.Idle;
    }

    public enum NPCState {Idle, Walking, Dialogue}
}
