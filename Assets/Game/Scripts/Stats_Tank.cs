using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;

public class Stats_Tank : MonoBehaviour
{
    [SerializeField] Player_Control control;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shieldBlock;
    [SerializeField] Material mt_shieldBlock;

    GameObject bp_shieldBlock;
    Unit stats;

    private float attackTimer;
    private float skillTimer;

    private bool skillRangeView;
    private bool turretMode;

    private void Awake()
    {
        stats = new Unit("Fortress");
        skillRangeView = false;
        turretMode = false;

        attackTimer = 0f;
        skillTimer = 10f;
    }
    private void Update()
    {
        attackTimer += Time.deltaTime;
        skillTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            turretMode = !turretMode;
            control.Hold();
        }

        if (stats.CheckCanDefaultAttack(attackTimer) && Input.GetMouseButtonDown(0) && !skillRangeView)
        {
            Default_Attack();
            attackTimer = 0f;
        }
        
        if (stats.CheckCanSkill(skillTimer) && Input.GetKeyDown(KeyCode.E))
        {
            skillRangeView = !skillRangeView;
            if (skillRangeView) bp_shieldBlock = Instantiate(shieldBlock);
            else Destroy(bp_shieldBlock);
        }
        if (skillRangeView)
        {
            LayerMask layer = 1 << LayerMask.NameToLayer("Road");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layer))
            {
                bp_shieldBlock.transform.position = hit.transform.position + Vector3.up * 2;
            }

            if (Input.GetMouseButtonDown(0))
            {
                skillRangeView = false;
                bp_shieldBlock.GetComponent<BoxCollider>().enabled = true;
                bp_shieldBlock.GetComponent<Renderer>().material = mt_shieldBlock;
                bp_shieldBlock = null;
                skillTimer = 0f;
            }
        }
    }

    public void Default_Attack()
    {
        var obj = Instantiate(bullet, control.head.transform.position, control.head.transform.rotation);
        obj.GetComponent<Bullet>().Setting(gameObject, stats.attackDamage, 20, 1);
    }

    
}
