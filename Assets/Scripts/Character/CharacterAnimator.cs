using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // Parameters
    public float MoveX {get; set;}
    public float MoveY {get; set;}
    public bool IsMoving{get;set;}

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;

    // References
    SpriteRenderer spriteRenderer;

    public float frameRate ;

    public FacingDirection DefaultDirection{
        get => defaultDirection;
    }
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = ScriptableObject.CreateInstance<SpriteAnimator>();
        walkDownAnim.Init(spriteRenderer, walkDownSprites, frameRate);
        walkUpAnim = ScriptableObject.CreateInstance<SpriteAnimator>();
        walkUpAnim.Init(spriteRenderer, walkUpSprites, frameRate);
        walkRightAnim = ScriptableObject.CreateInstance<SpriteAnimator>();
        walkRightAnim.Init(spriteRenderer, walkRightSprites, frameRate);
        walkLeftAnim = ScriptableObject.CreateInstance<SpriteAnimator>();
        walkLeftAnim.Init(spriteRenderer, walkLeftSprites, frameRate);
        SetFacingDirection(defaultDirection);
        //currentAnim = walkDownAnim;
    }

    private void Update() {

        var prevAnim = currentAnim;
        if(MoveX == 1) //player is moving RIGHT
            currentAnim = walkRightAnim;
        else if(MoveX == -1)
                currentAnim = walkLeftAnim;
             else if(MoveY == 1)
                    currentAnim = walkUpAnim;
                  else if(MoveY == -1)
                        currentAnim = walkDownAnim;
        
        if(currentAnim != prevAnim)
            currentAnim.Start();

        if(IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];
    }

    public void SetFacingDirection(FacingDirection dir){
        switch (dir)
        {  case FacingDirection.Right: 
            MoveX = 1;
            break;
            case FacingDirection.Left:
            MoveX = -1;
            break;
            case FacingDirection.Up:
            MoveY = 1;
            break;
            case FacingDirection.Down:
            MoveY = -1;          
            break;
        }

    }

}

public enum FacingDirection {Up, Down, Left, Right };