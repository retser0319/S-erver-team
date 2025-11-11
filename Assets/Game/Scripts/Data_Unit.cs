using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Unit
{
    public int attackDamage;
    public float attackSpeed;
    public int healthMax;
    public int health;
    int resistance;
    public float moveSpeed;

    public float skillCooltime;
    public Unit(string type)
    {
        attackDamage = 10;
        attackSpeed = 0.8f;
        healthMax = 300;
        health = healthMax;
        resistance = 0;
        moveSpeed = 10;
        skillCooltime = 10;
        if (type == "Fortress")
        {
            healthMax += 100;
            health = healthMax;
            resistance += 3;
        }
    }

    public bool CheckCanDefaultAttack(float at) 
    {
        if (at >= 1f / attackSpeed) return true;
        return false;
    }
    public bool CheckCanSkill(float st)
    {
        if (st > skillCooltime) return true;
        return false;
    }
}

public class Fortress : Unit
{
    Skill[] skill;

    public Fortress() : base("Defenser")
    {
        skill = new Skill[3];
    }
}
