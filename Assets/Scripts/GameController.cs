using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialogue, Cutscene };

public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;
    // Update is called once per frame
    
    private void Awake() {
        ConditionsDB.Init();
    }

    void Start(){
        playerController.OnEncountered += StartBattle;
        battleSystem.WonBattle += EndBattle;

        playerController.OnEnterTrainersView += (Collider2D trainerCollider) => {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if(trainer != null){
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

        DialogueManager.Instance.OnShowDialog += ()=>{
            state =  GameState.Dialogue;
            playerController.Animator.IsMoving = false;
        };
        DialogueManager.Instance.OnCloseDialog += ()=>{
            if(state == GameState.Dialogue)
                state =  GameState.FreeRoam;
        };
    }

    void StartBattle(){
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        var playerParty = playerController.GetComponent<PokemonParty>();
        MapArea area = FindObjectOfType<MapArea>(); //maybe a better implementation that considers area changings
        var wildPokemon = area.GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon); 
    }

    void EndBattle(bool won){
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);    
    }
    void Update()
    {
        switch (state)
        {
            case GameState.FreeRoam:
                playerController.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.Dialogue:
                DialogueManager.Instance.HandleUpdate();
                break;    
        }
    }
}
