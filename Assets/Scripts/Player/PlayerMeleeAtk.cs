using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMeleeAtk : MonoBehaviour
{
    [Header("Detecting targets in atk")]
    [SerializeField] private LayerMask target;
    [SerializeField] private Transform atkPosition;

    [Header("Animations control")]
    [SerializeField] private Slider delayBar;
    [SerializeField] private PlayerAnimationsAndPositions pAnim;
    bool readyToAtk;

    private void Start()
    {
        readyToAtk = true;
        StartCoroutine(DelayToAtk());
    }

    #region ATK animations
    IEnumerator DelayToAtk()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (readyToAtk)
            {
                readyToAtk = false;

                GameData gameData = ManagerData.Load();
                float atkSpeed = gameData.AtkSpeed;
                delayBar.maxValue = atkSpeed;
                delayBar.value = 0;

                while (delayBar.value < delayBar.maxValue)
                {
                    yield return new WaitForSeconds(.01f);
                    delayBar.value += .01f;
                }
                pAnim.IsAttacking = true;
                pAnim.PlayAnimAtk();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion


    #region Called in animAtk
    //Called in animation Base Layer.RightArm_Atk
    public void DetectingTargetsInAtk()
    {
        if (GetComponentInParent<PlayerStatus>().ImAlive)
        {
            GameData gameData = ManagerData.Load();

            RaycastHit2D[] hit = Physics2D.RaycastAll(atkPosition.position, atkPosition.right, gameData.RangeAtk, target);

            if (hit != null)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (Critical(gameData.CriticalDMG))
                    {
                        hit[i].collider.GetComponentInChildren<EnemyStatus>().
                            LoseLife(CriticalDMG(gameData.Damage, gameData.CriticalDMG), true);
                    }
                    else
                    {
                        hit[i].collider.GetComponentInChildren<EnemyStatus>().
                            LoseLife(SimpleDMG(gameData.Damage), false);
                    }
                }
            }
        }
    }
    
    public void EndAtk()
    {
        pAnim.IsAttacking = false;
        readyToAtk = true;
    }

    #endregion


    #region DMG Value
    bool Critical(float cri)
    {
        float value = Random.Range(0, 100);
        return value <= cri;
    }
    float CriticalDMG(float damage, float criticalDMG)
    {
        return damage + (damage * criticalDMG / 100);
    }
    float SimpleDMG(float damage)
    {
        return Random.Range(damage, damage * 1.2f);
    }
    private void OnDrawGizmos()
    {
        GameData gameData = ManagerData.Load();
        Debug.DrawRay(atkPosition.position, atkPosition.right * gameData.RangeAtk, Color.yellow);
    }

    #endregion
}
