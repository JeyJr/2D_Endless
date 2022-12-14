using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private float vit;
    [SerializeField] private float cri;
    [SerializeField] private float goldDrop;
    [SerializeField] private float skillDrop;
    public float Damage { get => atk * 1; }
    public float Defense { get => def / 2; }
    public float Life { get => life; }
    public float Critical { get => cri; }
    public float GoldDrop { get => goldDrop; }
    public float SkillDrop { get => skillDrop; }


    [Space(5)]
    [Header("Status")]
    [SerializeField] private float maxLife;
    [SerializeField] private float life;

    [Space(5)]
    [Header("Txt To Spawn")]
    public SpawnText spawnTextDMG;
    [SerializeField] private Transform spawnPosition;

    [Space(5)]
    [Header("Anim Control")]
    [SerializeField] private string enemyAnimName;
    public bool IsAttacking { get; set; }
    public string EnemyAnimName { get => enemyAnimName; }

    [Header("SpawnSkills")]
    [SerializeField] private List<GameObject> skillsToSpawn;

    [Space(5)]
    [Header("Others Control")]
    [SerializeField] private bool boss;
    [SerializeField] private bool enemyInTestZone;
    [SerializeField] private GameObject thisEnemyFullObj;
    bool isAlive;

    public bool IsAlive { get => isAlive; private set => isAlive = value; }

    private void Awake()
    {

        atk = Random.Range(atk, (atk + 1) * 2);
        def = Random.Range(def, (def + 1) * 2);
        vit = Random.Range(vit, (vit + 1) * 2);
        cri = Random.Range(cri, (cri + 1) * 2);

        maxLife = vit * 15;
        life = maxLife;
        IsAlive = true;
    }

    public void LoseLife(float dmg, bool critical)
    {
        float realDMG = dmg - ((dmg * Defense) / 100);
        spawnTextDMG.SpawnTextDamage(realDMG, critical);

        if (!enemyInTestZone)
            life -= realDMG;

        if (life <= 0 && !enemyInTestZone)
        {
            IsAlive = false;
            GetComponent<Animator>().Play($"Base Layer.{enemyAnimName}_Dead", 0);
        }
    }

    //this method is called on the last frame: Anim Dead;
    public void EnemyDrops()
    {
        if(Random.Range(0, 100) <= SkillDrop)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
            Instantiate
            (
                skillsToSpawn[Mathf.RoundToInt(Random.Range(0, skillsToSpawn.Count))], 
                pos, 
                Quaternion.identity
            );
        }

        GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().EnemyDead(GoldDrop, boss);
        Destroy(thisEnemyFullObj);
    }


    //Called on animations atk
    public void TargetDmg() => GetComponentInParent<EnemyBehavior>().SetTargetDamage(Damage, Critical);
    public void SetAwait(){
        IsAttacking = !IsAttacking;
        GetComponentInParent<EnemyBehavior>().SetAwait();
    }
}
