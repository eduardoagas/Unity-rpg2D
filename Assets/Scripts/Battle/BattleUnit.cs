using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isPlayerUnit;
    public Pokemon Pokemon {get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; }
    [field: SerializeField] public BattleHud Hud { get; private set; }
    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake(){
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    private void Start() {
        Pokemon.onSelfHit += PlayHitAnimation;
    }

    public void Setup(Pokemon pokemon){
        Pokemon = pokemon;
        if (IsPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;
        Hud.setData(pokemon);
        image.color = originalColor;
        PlayEnterAnimation();
    } 

    public BattleUnit returnUnit(){
        return this;
    }

    public void PlayEnterAnimation(){
        if (IsPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);
    
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    
    }

    public void PlayAttackAnimation(){
        var sequence = DOTween.Sequence();
        if(IsPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation(){
        Debug.Log("hit animation played.");
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation(){
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

}
