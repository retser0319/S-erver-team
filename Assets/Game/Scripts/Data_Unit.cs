using UnityEngine;

public class Unit
{
    int attackDamage;
    float attackSpeed;
    int healthMax;
    int health;
    int resistance;
    float moveSpeed;

    public Unit(string type)
    {
        attackDamage = 10;
        attackSpeed = 0.8f;
        healthMax = 300;
        health = 300;
        resistance = 0;
        moveSpeed = 10;
    }
}

public class Fortress : Unit
{
    Skill[] skill;

    public Fortress() : base("Defenser")
    {
        skill = new Skill[4];
    }
}
