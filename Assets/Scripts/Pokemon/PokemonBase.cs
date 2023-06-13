using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonBase", menuName = "PokeClone/Create New Pokémon", order = 0)]
public class PokemonBase : ScriptableObject {

    [SerializeField] new string name;

    [TextArea][SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base stats

    [SerializeField] int maxhp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;
    public string Description { get => Description; }
    public string Name { get => name; set => name = value; }
    public Sprite FrontSprite { get => frontSprite; set => frontSprite = value; }
    public Sprite BackSprite { get => backSprite; set => backSprite = value; }
    public PokemonType Type1 { get => type1; set => type1 = value; }
    public PokemonType Type2 { get => type2; set => type2 = value; }
    public int Maxhp { get => maxhp; set => maxhp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int SpAttack { get => spAttack; set => spAttack = value; }
    public int SpDefense { get => spDefense; set => spDefense = value; }
    public int Speed { get => speed; set => speed = value; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; set => learnableMoves = value; }
}

[System.Serializable]
public class LearnableMove{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase { get => moveBase; set => moveBase = value; }
    public int Level { get => level; set => level = value; }
}

public enum PokemonType{
    None,
    Normal,
    Fire,
    Grass,
    Water,
    Poison,
    Flying,
    Electric,
        
}

public enum Stat{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //not actual stats
    Accuracy,
    Evasion
}

public class TypeChart{

    static float [][] chart =
    {                   //  nor fir gra wat poi fly elc
     /*nor*/   new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
     /*fir*/   new float[] {1f, .5f, 2f, .5f, 1f, 1f, 1f},
     /*gra*/   new float[] {1f, .5f, .5f, 2f, .5f, 2f, 1f},
     /*wat*/   new float[] {1f, 2f, .5f, .5f, 1f, 1f, .5f},
     /*poi*/   new float[] {1f, 1f, 2f, 1f, .5f, 1f, 1f},
     /*fly*/   new float[] {1f, 1f, 2f, 1f, 1f, .5f,.5f},
     /*elc*/   new float[] {1f, 1f, .5f, 2f, 1f, 2f, .5f}
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType){
        if(attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;
        int row = (int) attackType - 1;
        int col = (int) defenseType -1;

        return chart[row][col];
              
    }

}
