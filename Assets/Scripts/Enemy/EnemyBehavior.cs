using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    EnemyStatus enemyStatus;
    Animator anim;

    [Header("Idle")]
    Task taskAwait;
    public bool waitLoopActions;
    public int delayToWait;

    [Header("MoveControl")]
    Task taskFindPlayer;
    public float mSpeed;
    Vector3 targetToMove;

    [Header("AtkControl")]
    Task taskAtkPlayer;
    bool findPlayerPosition, detectPlayerInRangeToAtk;

    [Header("Raycast")]
    public Transform atkPos;
    public float atkRange;
    public LayerMask target;


    private void Start()
    {
        enemyStatus = GetComponentInChildren<EnemyStatus>();
        anim = GetComponentInChildren<Animator>();


        delayToWait = Random.Range(delayToWait, delayToWait * 2);
        mSpeed = Random.Range(mSpeed, mSpeed * 2);
    }

    private void Update()
    {
        if (enemyStatus.IsAlive)
        {
            if (!enemyStatus.IsAttacking && !waitLoopActions)
            {
                MoveControl();
            }
            else if(enemyStatus.IsAttacking && !waitLoopActions)
            {
                AtkControl();
            }
        }
        else
        {
            PlayAnim("Dead");
            StopCoroutine(DetectPlayerRangeToAtk());
        }
    }

    //IDLE
    public async void SetAwait()
    {
        taskAwait = Wait();
        await taskAwait;
    }
    async Task Wait()
    {
        waitLoopActions = true;
        PlayAnim("Idle");
        await Task.Delay(delayToWait * 1000);
        waitLoopActions = false;
    }

    //MOVE
    async void MoveControl()
    {
        PlayAnim("Run");
        transform.position = Vector3.MoveTowards(transform.position, targetToMove, mSpeed * Time.deltaTime);

        if (!findPlayerPosition)
        {
            findPlayerPosition = true;
            taskFindPlayer = TaskFindPlayerPosition();
            await taskFindPlayer;
        }

        if (!detectPlayerInRangeToAtk)
        {
            detectPlayerInRangeToAtk = true;
            StartCoroutine(DetectPlayerRangeToAtk());            
        }
    }
    async Task TaskFindPlayerPosition()
    {
        targetToMove = new Vector3(
            GameObject.FindWithTag("Player").GetComponent<Transform>().position.x,
            transform.position.y,
            transform.position.z
            );

        if (transform.position.x > targetToMove.x)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            transform.localEulerAngles = new Vector3(0, 0, 0);

        await Task.Delay(1000);
        findPlayerPosition = false;
    }
   
    //ATK
    void AtkControl()
    {
        PlayAnim("Atk");
    }
    IEnumerator DetectPlayerRangeToAtk()
    {
        if (enemyStatus.IsAlive)
        {
            RaycastHit2D hit = Physics2D.Raycast(atkPos.position, atkPos.right, atkRange, target);

            if (hit.collider != null)
            {
                enemyStatus.IsAttacking = true;
            }

            yield return new WaitForSeconds(1);
            detectPlayerInRangeToAtk = false;
        }
    }
    public void SetTargetDamage(float dmg, float critical)
    {
        RaycastHit2D hit = Physics2D.Raycast(atkPos.position, atkPos.right, atkRange, target);
        
        if(hit.collider != null)
        {
            hit.collider.GetComponentInChildren<PlayerStatus>().LoseLife(dmg, critical);
        }
    }

    //Anims
    void PlayAnim(string name) => anim.Play($"Base Layer.{enemyStatus.EnemyAnimName}_{name}");
    
    private void OnDrawGizmos()
    {
        Debug.DrawRay(atkPos.position, atkPos.right * atkRange, Color.blue);
    }
}
