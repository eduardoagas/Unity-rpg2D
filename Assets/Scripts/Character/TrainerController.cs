using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    Character character;
    

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    private void Awake() {
        character = GetComponent<Character>();    
    }
    
    private void Start(){
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player){
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        var diff = player.transform.position - transform.position;
        diff -= diff.normalized;
        var moveVec = new Vector2(Mathf.Round(diff.x), Mathf.Round(diff.y));
        yield return character.Move(moveVec);

        //Show dialog
        StartCoroutine(DialogueManager.Instance.ShowDialog(dialogue, ()=>{
           GameController.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFovRotation(FacingDirection dir){
        float angle = 0f;
        switch (dir)
        {  case FacingDirection.Right: 
            angle = 90f;
            break;
            case FacingDirection.Left:
            angle = 270f;
            break;
            case FacingDirection.Up:
            angle = 180f;
            break;
            case FacingDirection.Down:
            angle = 0f;          
            break;
        }
        fov.transform.eulerAngles = new Vector3(0f,0f,angle);
    }
}
