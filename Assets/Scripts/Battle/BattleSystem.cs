using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum BattleState {Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver}
public enum BattleAction {Move, SwitchPokemon, UseItem, Run}
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    public event Action<bool> WonBattle;
    BattleState? prevState;
    BattleState? state;
    int currentAction;
    int currentMove;
    int currentMember;
    bool isRunning;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        isRunning = false;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle(){
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return (dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.")); //maybe change to dialog box?
        ActionSelection();
        playerUnit.Pokemon.onSelfHit = PlayerSelfHit;
        enemyUnit.Pokemon.onSelfHit = EnemySelfHit;
    }

    IEnumerator RunTurns(BattleAction playerAction){
        state = BattleState.RunningTurn;
        isRunning = true;
        if(playerAction == BattleAction.Move){
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //check who goes first
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            bool playerGoesFirst = true;
            if(enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = IsPlayerFirst();
            
            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;
        
            var secondPokemon = secondUnit.Pokemon;
            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if(state == BattleState.BattleOver) yield break;
            
            if(secondPokemon.Hp > 0){
            //second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if(state == BattleState.BattleOver) yield break;
            }         
        }else{
            if(playerAction == BattleAction.SwitchPokemon){
                var selectedPokemon =  playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            //Enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if(state == BattleState.BattleOver) yield break;
        }
        if(state != BattleState.BattleOver){
            yield return isRunning = false;
            ActionSelection();
        }
    }

    void ActionSelection(){
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection(){
        state = BattleState.MoveSelection;
        dialogBox.SetDialog("");
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    bool IsPlayerFirst(){
        if(playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed)
            return true;
        if(playerUnit.Pokemon.Speed == enemyUnit.Pokemon.Speed)
            if (UnityEngine.Random.Range(0, 2) != 0)
                 return true;
        return false;        
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon){
        while (pokemon.StatusChanges.Count > 0){
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);           
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit){
        
        if(faintedUnit.IsPlayerUnit){
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null){
                OpenPartyScreen();
            }else
                BattleOver(false);
        }else{
            BattleOver(true);
        }
    }

    void PlayerSelfHit(){
        StartCoroutine(SelfHitRoutin(playerUnit));
    }

    void EnemySelfHit(){
        StartCoroutine(SelfHitRoutin(enemyUnit));
    }
    public IEnumerator SelfHitRoutin(BattleUnit unit){
        yield return new WaitForSeconds(1f);
        unit.Pokemon.StatusChanges.Enqueue($"It hurts itself in its confusion.");
        yield return new WaitForSeconds(2f);
        unit.PlayHitAnimation();
        yield return new WaitForSeconds(1f);
        
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move){
        
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if(!canRunMove){
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHp();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.MoveName}");
        
        if(CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon)){
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            yield return new WaitForSeconds(1f);
            if(move.Base.MoveCategory == MoveCategory.Status){
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }else{  
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHp();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.Hp > 0){
                foreach (var secondary in move.Base.Secondaries){
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if(rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            } 

            if(targetUnit.Pokemon.Hp <= 0){
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted.");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        } else {
             yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed!");       
        }        
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit){        
        if(state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        //poison/burn dmg resolve
        sourceUnit.Pokemon.OnAfterTurn(); 
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHp();

        if(sourceUnit.Pokemon.Hp <= 0){
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget){

            //Stat Boosting
            if(effects.Boosts != null){
                if(moveTarget == MoveTarget.Self)
                    source.ApplyBoosts(effects.Boosts);
                else
                    target.ApplyBoosts(effects.Boosts);
            }            

            //Status Condition
            if(effects.Status != ConditionID.none){
                if(moveTarget == MoveTarget.Self)
                    source.SetStatus(effects.Status);
                else
                    target.SetStatus(effects.Status);
                
            } 

            //Volatile Status Condition
            if(effects.VolatileStatus != ConditionID.none){
                if(moveTarget == MoveTarget.Self)
                    source.SetVolatileStatus(effects.VolatileStatus);
                else
                    target.SetVolatileStatus(effects.VolatileStatus);
            }

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);

    }

    void BattleOver(bool won){
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        WonBattle(won);
    }

    void OpenPartyScreen(){
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);    
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target){
        
        if(move.Base.AlwaysHits)
            return true;
        
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] {1f, 4f/3f, 5f/3f, 2f, 7/3f, 8f/3f, 3f};
       

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[evasion];

        return UnityEngine.Random.Range(1,101) <= moveAccuracy;
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if(damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");
        
        if(damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");

        if(damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective...");
        
    }    

    public void HandleUpdate() {
        if(state == BattleState.ActionSelection){
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection){
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen){
            HandlePartyScreenSelection();
        }
    }

    void HandlePartyScreenSelection(){
        //try some Matrix maybe?
        if(Input.GetKeyDown(KeyCode.RightArrow))
                ++currentMember;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
                --currentMember;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
                currentMember += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
                currentMember -= 2;
        
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
    
        partyScreen.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.Z)){
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.Hp <= 0){
                partyScreen.SetMessageText("You can't send out a fainted Pokemon.");
                return;
            }
            if(selectedMember == playerUnit.Pokemon){
                partyScreen.SetMessageText("This Pokemon is already in battle!");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if(prevState == BattleState.ActionSelection){
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            } else{
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }else if(Input.GetKeyDown(KeyCode.X)){
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
 
    }
    IEnumerator SwitchPokemon(Pokemon newPokemon){
            if(playerUnit.Pokemon.Hp > 0){
                dialogBox.EnableActionSelector(false);
                yield return dialogBox.TypeDialog($"Come back, {playerUnit.Pokemon.Base.Name}");
                playerUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);
            }
            playerUnit.Setup(newPokemon);
            dialogBox.SetMoveNames(newPokemon.Moves);
            yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

            state = BattleState.RunningTurn;
        }

    void HandleMoveSelection(){
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
           
        }else if(Input.GetKeyDown(KeyCode.DownArrow)){
                if(currentMove < playerUnit.Pokemon.Moves.Count - 2)
                    currentMove += 2;
                    } 
        else if(Input.GetKeyDown(KeyCode.LeftArrow)){
                if(currentMove > 0)
                    --currentMove;
        
        }else if(Input.GetKeyDown(KeyCode.UpArrow)){
                if(currentMove > 1)
                    currentMove -= 2;
        }
        
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if(!isRunning) {
            if(Input.GetKeyDown(KeyCode.Z)){
                if (playerUnit.Pokemon.Moves[currentMove].PP == 0) return;
                isRunning = true;
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(RunTurns(BattleAction.Move));
            }else{
                if(Input.GetKeyDown(KeyCode.X)){
                    dialogBox.EnableMoveSelector(false);
                    dialogBox.EnableDialogText(true);
                    ActionSelection();
                }
            }
        }

    }

    void HandleActionSelection(){
        if(Input.GetKeyDown(KeyCode.RightArrow))
                ++currentAction;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
                --currentAction;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
                currentAction += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
                currentAction -= 2;
        
        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z)){
            if (currentAction == 0){
                //Fight
                MoveSelection();
            }else if (currentAction == 1)
            {
                //Bag
            }else if (currentAction == 2)
            {
                //Pokemon
                prevState = state;
                OpenPartyScreen();
            }else if (currentAction == 3)
            {
                //Run
            }
        }

    }

}
