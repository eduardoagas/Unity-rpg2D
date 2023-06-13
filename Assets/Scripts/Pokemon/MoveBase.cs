using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "PokeClone/Create New Move")]
public class MoveBase : ScriptableObject{
        [field: SerializeField] public string MoveName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public PokemonType Type { get; private set; }
        [field: SerializeField] public int Power { get; private set; }
        [field: SerializeField] public int Accuracy { get; private set; }
        [field: SerializeField] public bool AlwaysHits { get; private set; }
        [field: SerializeField] public int Pp { get; private set; }
         [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public MoveCategory MoveCategory { get; private set; }
        [field: SerializeField] public MoveEffects Effects{ get; private set; }
        [field: SerializeField] public List<SecondaryEffects> Secondaries { get; private set; }
        [field: SerializeField] public MoveTarget Target { get; private set; }
}   

[System.Serializable]
public class MoveEffects{
    [field: SerializeField] public List<StatBoost> Boosts{get; private set; }
    [field: SerializeField] public ConditionID Status {get; private set; }
    [field: SerializeField] public ConditionID VolatileStatus {get; private set; }

}

[System.Serializable]
public class SecondaryEffects : MoveEffects{
    [field: SerializeField] public int Chance {get; private set; }
    [field: SerializeField] public MoveTarget Target {get; private set; }
}

[System.Serializable]
public class StatBoost{
    public Stat stat;
    public int boost;
}
public enum MoveCategory { Physical, Special, Status};

public enum MoveTarget{ Foe, Self}