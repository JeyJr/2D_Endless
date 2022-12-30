using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMeleeAtk : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform atkPosition;

    public void Atk()
    {
        GameData gameData = ManagerData.Load();

        RaycastHit2D[] hit = Physics2D.RaycastAll(atkPosition.position, atkPosition.right, gameData.RangeAtk, enemyMask);

        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (Critical(gameData.CriticalDMG))
                {
                    hit[i].collider.GetComponent<EnemyStatus>().
                        LoseLife(CriticalDMG(gameData.Damage, gameData.CriticalDMG), true);
                }
                else
                {
                    hit[i].collider.GetComponent<EnemyStatus>().
                        LoseLife(SimpleDMG(gameData.Damage), false);
                }
            }
        }
    }



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

        //Gizmos.color = Color.yellow;

        //Vector3 pos = atkPosition.position;
        //pos.x += x;

        //Gizmos.DrawRay(pos, boxDirection * maxDistance);
        //Gizmos.DrawWireCube(pos + boxDirection * maxDistance, boxScale * 2);
    }
}
