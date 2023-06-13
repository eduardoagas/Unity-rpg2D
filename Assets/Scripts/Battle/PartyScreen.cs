using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    public void Init(){
        memberSlots = GetComponentsInChildren<PartyMemberUI>();       
    }

    public void SetPartyData(List<Pokemon> pokemons){
        
        this.pokemons = pokemons;

        for(int i=0; i < 6; i++){
            if(i < pokemons.Count)
                memberSlots[i].setData(pokemons[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }
        messageText.text = "Choose a Pokemon";
    }

    /*public void UpdateMemberSelection(int selected){
        for (int i = 0; i < pokemons.Count; i++){
            if(i == selected)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    } */   

    public void UpdateMemberSelection(int selected){
        for (int i = 0; i < pokemons.Count; i++){
            if(i == selected)
                memberSlots[i].SetSelectedHud(true);
            else
                memberSlots[i].SetSelectedHud(false);
        }
    } 

    public void SetMessageText(string message){
        messageText.text = message;
    }

}
