using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void MyAction();

public class Monster : MonoBehaviour, IBattle
{
    List<IBattle> myAttackers = new List<IBattle>();
    Transform _target = null;
    Transform myTarget
    {
        get => _target;
        set
        {
            _target = value;
            if (_target != null) _target.GetComponent<IBattle>()?.AddAttacker(this);
        }
    }
    Color orgColor = Color.white;
    Vector3 startPos = Vector3.zero;
    Coroutine moveCo = null;
    Coroutine rotCo = null;
    public CharacterStat myStat;

    //컴포넌트
    Animator _anim;
    Renderer _render;

    protected void AttackTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(AttackingTarget(target, myStat.AttackRange, myStat.AttackDelay));
    }

    public enum STATE
    {
        Create, Idle, Roaming, Battle, Dead
    }
    public STATE myState = STATE.Create;

    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                StartCoroutine(DelayRoaming(2.0f));
                break;
            case STATE.Roaming:
                Vector3 pos = Vector3.zero;
                pos.x = Random.Range(-15.0f, 15.0f);
                pos.z = Random.Range(-15.0f, 15.0f);
                pos = startPos + pos;
                MoveToPosition(pos, () => ChangeState(STATE.Idle));
                break;
            case STATE.Battle:
                AttackTarget(myTarget);
                break;
            case STATE.Dead:
                StopAllCoroutines();
                GetComponent<CapsuleCollider>().isTrigger = true;
                _anim.SetTrigger("Dead");
                foreach(IBattle ib in myAttackers)
                {
                    ib.DeadMessage(transform);
                }
                StartCoroutine(DisApearing(6.0f, 2.0f));
                break;
        }
    }

    void StateProcess()
    {
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                break;
            case STATE.Roaming:
                break;
            case STATE.Battle:
                break;
            case STATE.Dead:
                break;
        }
    }

    IEnumerator DelayRoaming(float t)
    {
        yield return new WaitForSeconds(t);
        ChangeState(STATE.Roaming);
    }

    void MoveToPosition(Vector3 pos, MyAction done = null, bool Rot = true)
    {
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            moveCo = null;
        }
        moveCo = StartCoroutine(MovingToPosition(pos, done));

        if (Rot)
        {
            if (rotCo != null)
            {
                StopCoroutine(rotCo);
                rotCo = null;
            }
            rotCo = StartCoroutine(RotatingToPosition(pos));
        }
    }

    IEnumerator MovingToPosition(Vector3 pos, MyAction done)
    {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        //달리기 시작
        _anim.SetBool("IsMoving", true);
        while (dist > 0.0f)
        {
            if (_anim.GetBool("IsAttacking"))
            {
                _anim.SetBool("IsMoving", false);
                yield break;
            }

            if (!_anim.GetBool("IsAttacking"))
            {
                float delta = myStat.MoveSpeed * Time.deltaTime;
                if (delta > dist)
                {
                    delta = dist;
                }
                dist -= delta;
                transform.Translate(dir * delta, Space.World);
            }
            yield return null;
        }
        //달리기 끝 - 도착
        _anim.SetBool("IsMoving", false);
        done?.Invoke();
    }

    IEnumerator RotatingToPosition(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float Angle = Vector3.Angle(transform.forward, dir);
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -rotDir;
        }

        while (Angle > 0.0f)
        {
                float delta = myStat.RotSpeed * Time.deltaTime;
                if (delta > Angle)
                {
                    delta = Angle;
                }
                Angle -= delta;
                transform.Rotate(Vector3.up * rotDir * delta, Space.World);
            yield return null;
        }
    }

    IEnumerator AttackingTarget(Transform target, float AttackRange, float AttackDelay)
    {
        float playTime = 0.0f;
        float delta = 0.0f;
        while(target != null)
        {
            if(!_anim.GetBool("IsAttacking")) playTime += Time.deltaTime;
            //이동
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude;
            if (dist > myStat.AttackRange)
            {
                _anim.SetBool("IsMoving", true);
                dir.Normalize();
                delta = myStat.MoveSpeed * Time.deltaTime;
                if (delta > dist)
                {
                    delta = dist;
                }
                transform.Translate(dir * delta, Space.World);
            }
            else
            {
                _anim.SetBool("IsMoving", false);
                if (playTime >= myStat.AttackDelay)
                {
                    //공격
                    playTime = 0.0f;
                    _anim.SetTrigger("Attack");
                }
            }
            //회전
            delta = myStat.RotSpeed * Time.deltaTime;
            float Angle = Vector3.Angle(dir, transform.forward);
            float rotDir = 1.0f;
            if (Vector3.Dot(transform.right, dir) < 0.0f)
            {
                rotDir = -rotDir;
            }
            if (delta > Angle)
            {
                delta = Angle;
            }
            transform.Rotate(Vector3.up * delta * rotDir, Space.World);
            yield return null;
        }
        _anim.SetBool("IsMoving", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponentInChildren<Renderer>();
        orgColor = _render.material.color;
        startPos = transform.position;
        _anim = GetComponent<Animator>();
        ChangeState(STATE.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();   
    }

    public void FindTarget(Transform target)
    {
        if (myState == STATE.Dead) return;
        myTarget = target;
        StopAllCoroutines();
        ChangeState(STATE.Battle);
    }

    public void LostTarget()
    {
        if (myState == STATE.Dead) return;
        myTarget = null;
        StopAllCoroutines();
        _anim.SetBool("IsMoving", false);
        ChangeState(STATE.Idle);
    }

    public void AttackTarget()
    {
        myTarget.GetComponent<IBattle>()?.OnDamage(myStat.AP);
    }

    public void OnDamage(float dmg)
    {
        myStat.HP -= dmg;
        /*while (OnDamage(true))
        {
            myTarget.GetComponent<Host>().AttackSound.Play();
        }*/
        if (Mathf.Approximately(myStat.HP, 0.0f))
        {
            ChangeState(STATE.Dead);
        }
        else
        {
            _anim.SetTrigger("Damage");
            StartCoroutine(DamagingColor(Color.red, 0.5f));
        }
    }

    public bool IsLive()
    {
        return myState != STATE.Dead;
    }

    public void AddAttacker(IBattle ib)
    {
        myAttackers.Add(ib);
    }

    public void DeadMessage(Transform tr)
    {
        if (tr == myTarget)
        {
            LostTarget();
            Destroy(gameObject);
        }
    }

    IEnumerator DisApearing(float d, float t)
    {
        yield return new WaitForSeconds(t);
        float dist = d;
        while (dist > 0.0f)
        {
            float delta = 0.5f * Time.deltaTime;
            if (delta > dist)
            {
                delta = dist;
            }
            dist -= delta;
            transform.Translate(Vector3.down * delta, Space.World);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator DamagingColor(Color color, float t)
    {
        GetComponentInChildren<Renderer>().material.color = color;
        yield return new WaitForSeconds(t);
        GetComponentInChildren<Renderer>().material.color = orgColor;
    }
}