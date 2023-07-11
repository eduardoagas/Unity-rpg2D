using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialogue, Cutscene, Paused };

public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    public static GameController Instance{get;private set;}
    GameState state;
    GameState stateBeforePause;
    // Update is called once per frame
    
    private void Awake() {
        Instance = this;
        ConditionsDB.Init();
    }

    void Start(){
        //playerController.OnEncountered += StartBattle;
        battleSystem.WonBattle += EndBattle;

        DialogueManager.Instance.OnShowDialog += ()=>{
            state =  GameState.Dialogue;
            playerController.Animator.IsMoving = false;
        };
        DialogueManager.Instance.OnCloseDialog += ()=>{
            if(state == GameState.Dialogue)
                state =  GameState.FreeRoam;
        };
    }

    public void PauseGame(bool pause){
        if(pause){
            stateBeforePause = state;
            state = GameState.Paused;
        }else{
            state = stateBeforePause;
        }
    }

    public void StartBattle(){
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        var playerParty = playerController.GetComponent<PokemonParty>();
        MapArea area = FindObjectOfType<MapArea>(); //maybe a better implementation that considers area changings
        var wildPokemon = area.GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon); 
    }

    public void StartTrainerBattle(TrainerController trainerController){
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainerController.GetComponent<PokemonParty>();
        MapArea area = FindObjectOfType<MapArea>(); 
        battleSystem.StartTrainerBattle(playerParty, trainerParty); 
    }

    public void OnEnterTrainersView(TrainerController trainerController){
        state = GameState.Cutscene;
        StartCoroutine(trainerController.TriggerTrainerBattle(playerController));
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
