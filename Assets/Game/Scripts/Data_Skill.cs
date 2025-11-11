using UnityEngine;

public class Skill
{
    Transform caster; // 사용자
}

public class Passive
{

    public virtual void Effect()
    {

    }
}

public class Active
{
    Vector3 position; // 사용위치

    public virtual void Effect(GameObject bullet)
    {

    }
}

public class Fortress_Attack : Active
{
    public override void Effect(GameObject bullet)
    {
        
    }
}