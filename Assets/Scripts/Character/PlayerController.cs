using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    public event Action OnEncountered;    
    public event Action<Collider2D> OnEnterTrainersView;
    private bool isMoving;
    private Vector2 input;
    //private Animator animator;
    private Vector2 lastPos;
    public bool changedDir;
    
    

    private Character character;

    public CharacterAnimator Animator{
        get{return character.Animator;}
    }
    private void Awake() {

        character = GetComponent<Character>();
    }
    private void Start(){
        lastPos = transform.position;
        changedDir = false;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.isMoving){

             /*if (Input.GetKeyDown(KeyCode.X)) {
                 moveSpeed *= 2;
                 animator.frameRate *=2;
             }
             else if (Input.GetKeyUp(KeyCode.X)){
                moveSpeed /= 2;
                 animator.frameRate /=2;
             }*/


            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //remove diagonal movement and smooth directional control
            if(input.x == 0 || input.y == 0){
                changedDir = false;
            }
            if(input.x != 0 && input.y != 0 && !changedDir){
                        if(lastPos.x != transform.position.x)//ultimo passo foi horizontal
                            input.x = 0; //foi pra vertical
                        if(lastPos.y != transform.position.y)
                            input.y = 0;
                        changedDir = true;
            }
            if(input.x != 0 && input.y != 0 && changedDir){
                        if(lastPos.x != transform.position.x)//ultimo passo foi horizontal
                            input.y = 0; //continua na horizontal
                        if(lastPos.y != transform.position.y)
                            input.x = 0;
            }       

            if (input != Vector2.zero){
                
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if(Input.GetKeyDown(KeyCode.Z)){
            Interact();
        }
    }

    /*IEnumerator Move(Vector3 targetPos)
    {   
        lastPos = transform.position;
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);  
            yield return null;
        }
        /*a while loop will execute very fast, thousands of times 
    per second. So putting it inside a normal while loop will 
    complete it instantly. Adding a yield return null will stop
     the while loop and continue it in the next frame. 
     So the loop will only execute once per frame and our 
     player will move to the target position over some time.*/
    /*
        transform.position = targetPos; //once you deal with Mathf.Epsilon
        isMoving = false;

        CheckForEncounters();
        
    }*/

    void Interact(){
        //var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var facingDir = new Vector3(Animator.MoveX, Animator.MoveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
    
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null){
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    /*private bool isWalkable(Vector3 targetPos){
        return !(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) != null);
    }*/

    private void CheckForEncounters(){
        if(Physics2D.OverlapCircle(transform.position-new Vector3(0, 0.5f, 0), 0.2f, GameLayers.i.GrassLayer) != null){
            //grassB.transform.position = transform.position-new Vector3(0, 0.3f, 0);
            if(UnityEngine.Random.Range(1,101) <= 10){
                //animator.SetBool("isMoving", false);
                Animator.IsMoving = false;
                OnEncountered(); //funções de outras classes serão adicionadas
            }

        }
    }

    private void CheckIfInTrainersView(){
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if(collider != null){
            Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);

        }    
    }

    private void OnMoveOver() {
        //CheckForEncounters();
        CheckIfInTrainersView();
    }


}
