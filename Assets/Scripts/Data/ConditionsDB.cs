using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{

    public static void Init(){
        foreach(var kvp in Conditions){
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions {get; set;} = new Dictionary<ConditionID, Condition>(){
        {ConditionID.psn,
            new Condition(){
                Name = "Poison",
                StartMessage = "has been poisoned!",
                OnAfterTurn = (Pokemon pokemon) =>{
                    pokemon.UpdateHp(pokemon.Maxhp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by poison!");
                }
            }
        },
        {ConditionID.brn,
            new Condition(){
                Name = "Burn",
                StartMessage = "has been burned!",
                OnAfterTurn = (Pokemon pokemon) =>{
                    pokemon.UpdateHp(Mathf.Max(pokemon.Maxhp / 16, 1));
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by burn!");
                }
            }
        },
        {ConditionID.par,
            new Condition(){
                Name = "Paralyze",
                StartMessage = "has been paralyzed!",
                OnBeforeMove = (Pokemon pokemon) =>{
                    var ran = Random.Range(1,5);
                    if(ran == 1){
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and can't move!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {ConditionID.frz,
            new Condition(){
                Name = "Freeze",
                StartMessage = "has been frozen!",
                OnBeforeMove = (Pokemon pokemon) =>{
                    var ran = Random.Range(1,5);
                    if(ran == 1){
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s not frozen anymore!");
                        return true;
                    }
                    return false;
                }
            }
        },
        {ConditionID.slp,
            new Condition(){
                Name = "Sleep",
                StartMessage = "fell asleep!",
                OnStart = (Pokemon pokemon) =>{
                    //sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"asleep for {pokemon.StatusTime} turns.");
                },
                OnBeforeMove = (Pokemon pokemon) =>{
                    if(pokemon.StatusTime <= 0){
                       pokemon.CureStatus();
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} woke up!");
                       return true;
                    } 
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping...");
                    return false;
                }
            }
        },
        {ConditionID.confusion,
            new Condition(){
                Name = "Confusion",
                StartMessage = "has been confused!",
                OnStart = (Pokemon pokemon) =>{
                    //confused for 1-4 turns
                    //pokemon.VolatileStatusTime =  10;
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"confused for {pokemon.VolatileStatusTime} turns.");
                },
                OnBeforeMove = (Pokemon pokemon) =>{
                    if(pokemon.VolatileStatusTime <= 0){
                       pokemon.CureVolatileStatus();
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} kicked out of confusion!");
                       return true;
                    } 
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.name} is confused...");
                    //%50 chande of doing a move
                    if(Random.Range(1,3) == 1)
                        return true;
                    //hurt by confusion
                    pokemon.SelfHit();
                    pokemon.UpdateHp(pokemon.Maxhp/8);
                    return false;
                
                }
            }
        }
    };


}

public enum ConditionID{ none, psn, brn, slp, par, frz, confusion}