using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public struct CharacterStat
{    
    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float ap;
    [SerializeField] float attackRange;
    [SerializeField] float attackDelay;

    public UnityAction<float> changeHp;
    public float HP 
    { 
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0.0f, maxHp);
            changeHp?.Invoke(hp / maxHp);
        }
    }
    public float MaxHp
    {
        get => maxHp;
        set => maxHp = value;
    }
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    public float RotSpeed
    {
        get => rotSpeed;
        set => rotSpeed = value;
    }
    public float AP
    {
        get => ap;
        set => ap = value;
    }
    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }
    public float AttackDelay
    {
        get => attackDelay;
        set => attackDelay = value;
    }
}
