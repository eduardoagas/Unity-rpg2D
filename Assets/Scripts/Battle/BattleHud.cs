using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;
    Dictionary<ConditionID, Color> statusColors; //use dics to use something else than number as keys
    
    public void setData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.setHp((float) pokemon.Hp / pokemon.Maxhp);

        statusColors = new Dictionary<ConditionID, Color>(){
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.par, parColor},
            {ConditionID.slp, slpColor},
            {ConditionID.frz, frzColor}
        };

        SetStatusText();
        _pokemon.onStatusChanged += SetStatusText;
    }

    void SetStatusText(){
        if(_pokemon.Status == null)
            statusText.text = "";
        else{
            statusText.text = _pokemon.Status.ID.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.ID];
        }
    }

    public IEnumerator UpdateHp(){
        if(_pokemon.HpChanged){
            yield return hpBar.SetHpSmooth((float) _pokemon.Hp / _pokemon.Maxhp);
            _pokemon.HpChanged = false;
        }
    }
}
