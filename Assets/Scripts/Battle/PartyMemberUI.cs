using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Color highlightColor;
    [SerializeField] Color hudSelectedColor;

    Pokemon _pokemon;
    
    
    Image image;
    Color originalColor;
    private void Awake(){
        image = GetComponent<Image>();
        originalColor = image.color;
    }
    public void setData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.setHp((float) pokemon.Hp / pokemon.Maxhp);
    }

    public void SetSelected(bool selected){
        if(selected){
            nameText.color = highlightColor;
            levelText.color = highlightColor;
        }
        else{
            nameText.color = Color.black;
            levelText.color = Color.black;
        }
    }

    public void SetSelectedHud(bool selected){
        SetSelected(selected);
        if(selected)
            image.color = hudSelectedColor;
        else
            image.color = originalColor;
    }


}
