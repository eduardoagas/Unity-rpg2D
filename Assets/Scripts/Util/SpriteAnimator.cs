using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : ScriptableObject
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    [SerializeField] public float FrameRate{get; set;}
    int currentFrame;
    float timer;
    public float deltaTime;
    public List<Sprite> Frames{
        get {return frames;}
    }
    //public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate = 0.16f)
    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.FrameRate = frameRate;
    }

    public void Init(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.FrameRate = frameRate;
    }

    public void Start(){
        currentFrame = 1;
        timer = 0f;
        spriteRenderer.sprite = frames[1];
    }
    public void HandleUpdate(){
        timer += Time.deltaTime;
        if(timer > FrameRate){
            currentFrame = (currentFrame + 1) % frames.Count; // % = mod
            spriteRenderer.sprite = frames[currentFrame];
            timer -= FrameRate;
        //ShowFrameRate();
        }

    /*void ShowFrameRate () {
         deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
         float fps = 1.0f / deltaTime;
         Debug.Log($"FPS = {Mathf.Ceil (fps)}");
     }*/
    }

}
