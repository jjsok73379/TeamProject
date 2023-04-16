using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnDamage(float dmg);
    bool IsLive();
    void AddAttacker(IBattle ib);
    void DeadMessage(Transform tr);
}

public class BattleSystem : MonoBehaviour
{

}
