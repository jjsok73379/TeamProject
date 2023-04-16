using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Unity.Jobs;
using UnityEngine.SearchService;
using static UnityEngine.GraphicsBuffer;
using System.Threading;
using UnityEngine.UI;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine.AI;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class Host : MonoBehaviour, IBattle
{
    public AudioSource AttackSound;

    public GameObject Icon;

    public bool LineChk = false; // 퇴장하는 Npc와 입장하는 Npc의 충돌시 시계아이콘 생성되는 오류 수정
    public static Host inst = null;

    public GameObject Quest; // 프리팹 둘은 연결 >> 퀘스트 완료 >> 완료되었으면 quest 프리팹을 파괴
    // 화살 위치 화살
    public GameObject myBow;
    public GameObject orgArrow;

    //마법 이펙트 
    public GameObject magiceff;
    public Transform trs;

    //NpcClock,QuestImoticon
    public Transform spawnPoints;
    public Transform myIconZone;
    public Transform OutPoint;
    public GameObject myStaff;

    public GameObject IconArea = null;
    public bool VLchk; // 마을사람과 모험가 구분용
    public bool Questing = false; // 현재 퀘스트 진행중 = 재방문 해야하는 Npc
    public bool exitchk = false;
    public bool Clockchk = false;
    public bool IsQuest = false;
    int count; //배열 체크

    public bool onAngry = false;
    public bool onSmile = false;

    public int People; // 사람 구분

    public int FarmAni;
    //네비게이션
    NavMeshAgent agent;
    [SerializeField] Vector3 lob; // 로비 도착지점
    [SerializeField] Vector3 res; // 식당 도착지점
    [SerializeField] Vector3 mot; // 여관 도착지점
    [SerializeField] Vector3 tel; // 텔레포트 위치

    [SerializeField] Transform Exit;

    ChairBedChk bedchairvalue;
    public int purpose; // 방문목적
    public LayerMask layerMask;

    //컴포넌트
    Animator _anim;
    PubMotelIcon _PM;
    ADNpc _adnpc;
    ClockIcon _Clock;


    public enum STATE
    {
        Create, Idle, Moving, Wait, Order, Eating, Sleeping, Battle, Exit, Farming
    }
    public STATE myState = STATE.Create;
    //자동 전투 관련
    public CharacterStat myStat;
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

    //위까지 전투 관련
    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case STATE.Create:
                /*if(GetComponent<ADNpc>().myStat.npcJob == CharState.NPCJOB.ACHER)
                {
                    myBow.SetActive(false);
                }*/
                break;
            case STATE.Idle:
                StopAllCoroutines();
                break;
            case STATE.Moving: // 윤섭
                StopAllCoroutines();
                agent.ResetPath(); // Wait 상태에서 Moving으로 바뀔때 목적지를 초기화
                _anim.SetBool("IsMoving", true);
                switch (purpose)
                {
                    case 0:
                        agent.SetDestination(lob);
                        break;
                    case 1:
                        agent.SetDestination(res);
                        break;
                    case 2:
                        agent.SetDestination(mot);
                        break;
                }
                StartCoroutine(ForwardCheck());
                break;
            case STATE.Wait:
                StopAllCoroutines(); // 모든 코루틴 멈추고 대기
                agent.ResetPath();
                //시계 생성
                if (LineChk == true && Clockchk == false) // 줄 서있는 상태 + 이미 생성된 시계가 없다면
                {
                    IconArea = UIManager.Inst.IconArea;
                    OnIcon("ClockIcon");
                    Clockchk = true;
                }
                StartCoroutine(ForwardCheck());
                break;
            case STATE.Order:
                StopAllCoroutines();
                LineChk = true; // 줄 서있는 상태
                if (Clockchk == false) // Wait 없이 바로 Order로 진입했다면 시계를 생성 = 이미 생성된 시계가 없다면
                {
                    IconArea = UIManager.Inst.IconArea;
                    OnIcon("ClockIcon");
                }
                Destroy(_Clock.myNotouch); // 시계 노터치 비활성화
                break;
            case STATE.Eating:
                StopAllCoroutines();
                OnIcon("EatIcon");
                _anim.SetTrigger("IsSeating");
                Invoke("GoExit", 5); //수정 종찬
                break;
            case STATE.Sleeping:
                StopAllCoroutines();
                OnIcon("SleepIcon");
                _anim.SetTrigger("IsLaying");
                Invoke("GoExit", 5);
                break;
            case STATE.Battle:
                AttackTarget(myTarget);
                break;
            case STATE.Farming:
                if (FarmAni == 0)
                {
                    _anim.SetTrigger("Pick");
                }
                else if (FarmAni == 1)
                {
                    _anim.SetTrigger("Find");
                }
                else if (FarmAni == 2)
                {
                    _anim.SetTrigger("Pick");
                }
                break;
            case STATE.Exit:
                StopAllCoroutines();
                LineChk = false; // 줄 서있는 상태가 아님
                // 퇴장하는 호스트는 입장하는 호스트를 피해감 (우선순위 조절)
                agent.avoidancePriority = 51;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

                _anim.SetBool("IsMoving", true);

                if (purpose != 0) // 방문목적이 여관or식당 이라면
                {
                    Destroy(Icon); // 잠 또는 식사 아이콘 삭제
                }

                if (onAngry == false) // 주문이 받아 들여졌을 때(화가 안났을 때)
                {
                    agent.ResetPath(); // 네비 초기화

                    switch (purpose)
                    {
                        case 0:
                            break;
                        case 1: //침대 초기화
                            bedchairvalue._chairSlot[count] = ChairBedChk.ChairSlot.None;
                            _anim.SetBool("SitToStand", true);
                            break;
                        case 2: //의자 초기화
                            bedchairvalue._bedSlot[count] = ChairBedChk.BedSlot.None;
                            _anim.SetBool("LayToStand", true);
                            break;
                    }
                }
                else // 화가 난 상태라면
                {
                    OnIcon("AngryIcon");
                }

                if (onSmile == true)
                {
                    OnIcon("SmileIcon");
                }

                agent.SetDestination(Exit.position); // outpoint로 가는 코루틴
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
            case STATE.Moving:
                if (purpose == 0)
                {
                    if (IsQuest) transform.SetAsFirstSibling();
                }
                break;
            case STATE.Wait:
                if (onAngry == true)
                {
                    ChangeState(STATE.Exit);
                }
                break;
            case STATE.Order:
                if (onAngry == true || onSmile == true)
                {
                    ChangeState(STATE.Exit);
                }
                break;
            case STATE.Battle:
                StopCoroutine(ForwardCheck());
                break;
            case STATE.Farming:
                break;
            case STATE.Exit:
                transform.SetAsLastSibling();
                break;
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        bedchairvalue = FindObjectOfType<ChairBedChk>();
        inst = this;
    }
    void Start()
    {
        _anim = GetComponent<Animator>();
        _adnpc = GetComponent<ADNpc>();
        ChangeState(STATE.Moving);
    }
    void Update()
    {
        StateProcess();
    }
    public void GoBed() //주문하고 침대로 이동
    {
        for (count = 0; count < bedchairvalue._bedSlot.Count; count++)
        {
            if (bedchairvalue._bedSlot[count] == ChairBedChk.BedSlot.None)
            {
                _anim.SetBool("IsMoving", true);
                agent.SetDestination(bedchairvalue._gobed[count]);
                StartCoroutine(BedToSleeping());
                bedchairvalue.bedSlot[count] = ChairBedChk.BedSlot.Check;
                break;
            }
        }
    }
    public void GoTable() //주문하고 테이블로 이동
    {
        for (count = 0; count < bedchairvalue._chairSlot.Count; count++)
        {
            if (bedchairvalue._chairSlot[count] == ChairBedChk.ChairSlot.None)
            {
                _anim.SetBool("IsMoving", true);
                agent.SetDestination(bedchairvalue._gotable[count]);
                StartCoroutine(EatToEating());
                bedchairvalue._chairSlot[count] = ChairBedChk.ChairSlot.Check;
                break;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 7.0f, 7.0f);
    }
    IEnumerator ForwardCheck()// 앞에 누가있는지 구분하여 상태를 Wait 또는 Order로 바꿔줌
    {
        while (true)
        {
            if (Physics.SphereCast(transform.position, 7.0f, transform.forward, out RaycastHit hitinfo, 7.0f, layerMask)) // 내 앞에 누군가 있을 경우
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero; //가속도 = 0
                _anim.SetBool("IsMoving", false); // 걷는 애니 중지

                if (hitinfo.collider.gameObject.layer == 6) // layer 6 = Host, layer 3 = Staff // 앞에 Host가 있을 경우
                {
                    if (hitinfo.collider.gameObject.GetComponent<Host>().LineChk == true)
                    {
                        LineChk = true;
                    }
                    ChangeState(STATE.Wait);
                }
                else // 앞에 Staff가 있을 경우
                {
                    myStaff = hitinfo.collider.gameObject; // myStaff를 각 구역에 맞게 설정해줌 ( 스마일 아이콘 발동 시키기 위해 )
                    ChangeState(STATE.Order); // Order로 상태변화
                }
            }
            else // 앞에 있는 누군가가 비켰을 경우
            {
                ChangeState(STATE.Moving);
            }
            yield return null;
        }
    }
    IEnumerator EatToEating() //도착지점에 도착하면 Eat상태에서 Eating상태로
    {
        while (true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= 1.0f)
                {
                    agent.velocity = Vector3.zero;
                    ChangeState(STATE.Eating);
                    agent.ResetPath();
                    if (count % 2 == 0)
                    {
                        transform.rotation = Quaternion.Euler(0, 270, 0);
                    }
                    else if (count % 2 == 1)
                    {
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator BedToSleeping() //도착지점에 도착하면 Eat상태에서 Eating상태로
    {
        while (true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= 1.0f)
                {
                    agent.velocity = Vector3.zero;
                    ChangeState(STATE.Sleeping);
                    agent.ResetPath();
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                    if (count > 3)
                    {
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
            yield return null;
        }
    }
    IEnumerator FinishQuest(float t) // 재방문
    {
        yield return new WaitForSeconds(t);
        _adnpc.AI_Per.SetActive(false);
        if (_adnpc.myStat.npcJob == CharState.NPCJOB.ACHER) myBow.SetActive(false);
        SpawnManager.Instance.Teleport(gameObject);
        _anim.SetBool("IsMoving", true);
        Clockchk = false;
        onSmile = false;
        ChangeState(STATE.Moving);
    }
    public void GoExit() //먹는상태에서 나가는 상태로
    {
        ChangeState(STATE.Exit);
    }
    public void StartFinishQuest()
    {
        StartCoroutine(FinishQuest(3.0f));
    }

    // 아이콘 관련 함수 또는 코루틴
    IEnumerator CoIcon(string IconName, float WaitSeconds)
    {
        yield return new WaitForSeconds(WaitSeconds);

        OnIcon(IconName);

        StopCoroutine(CoIcon(IconName, WaitSeconds));
    }
    public void CorourineIcon(string IconName, float WaitSeconds)
    {
        StartCoroutine(CoIcon(IconName, WaitSeconds));
    }
    public void OnIcon(string IconName)
    {
        Icon = Instantiate(Resources.Load($"IconPrefabs/{IconName}"), IconArea.transform) as GameObject;
        switch (IconName)
        {
            case "ClockIcon":
                _Clock = Icon.GetComponent<ClockIcon>();
                _Clock.myIconZone = myIconZone;
                _Clock.myHost = this.gameObject;
                Clockchk = true;
                break;
            case "SmileIcon":
                Icon.GetComponent<MoodIcon>().myIconZone = myIconZone;
                break;
            case "AngryIcon":
                Icon.GetComponent<MoodIcon>().myIconZone = myIconZone;
                if (SpawnManager.Instance.EndTime < TimeManager.Instance.DeadLine)
                {
                    if (People == 0)
                    {
                        GameManager.Instance.Fame -= GetComponent<VLNpc>().myQuest.rewardfame;
                    }
                    else
                    {
                        if (purpose == 0)
                        {
                            GameManager.Instance.Fame -= GetComponent<QuestInformation>().myQuest.rewardfame;
                        }
                        else
                        {
                            GameManager.Instance.Fame -= 10;
                        }
                    }
                }
                break;
            case "QuestIcon":
                Icon.GetComponent<QuestIcon>().myIconZone = myIconZone;
                break;
            case "BedIcon":
                _PM = Icon.GetComponent<PubMotelIcon>();
                _PM.myIconZone = myIconZone;
                _PM.myHost = this.gameObject;
                Icon.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(GoBed);
                break;
            case "MeatIcon":
                _PM = Icon.GetComponent<PubMotelIcon>();
                _PM.myIconZone = myIconZone;
                _PM.myHost = this.gameObject;
                Icon.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(GoTable);
                break;
            case "SleepIcon":
                _PM = Icon.GetComponent<PubMotelIcon>();
                _PM.myIconZone = myIconZone;
                break;
            case "EatIcon":
                _PM = Icon.GetComponent<PubMotelIcon>();
                _PM.myIconZone = myIconZone;
                break;
        }
    }
    //자동전투
    protected void AttackTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(AttackingTarget(target, myStat.AttackRange, myStat.AttackDelay));
    }
    public void OnDamage(float dmg)
    {
        myStat.HP -= dmg;
        if (Mathf.Approximately(myStat.HP, 0.0f))
        {
            StopAllCoroutines();
            foreach (IBattle ib in myAttackers)
            {
                ib.DeadMessage(transform);
            }
            _anim.SetTrigger("Dead");
            StartCoroutine(FinishQuest(2.0f));
        }
        else
        {
            _anim.SetTrigger("Damage");
        }
    }

    IEnumerator AttackingTarget(Transform target, float AttackRange, float AttackDelay)
    {
        float playTime = 0.0f;
        float delta = 0.0f;
        while (target != null)
        {
            if (!_anim.GetBool("IsAttacking")) playTime += Time.deltaTime;
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

                    switch (_adnpc.adtype)//직업별 애니메이션 실행
                    {
                        case 0: // 여자 궁수
                            _anim.SetTrigger("ArrowAttack");
                            CreateArrow();
                            break;
                        case 1: // 여자 도적
                            _anim.SetTrigger("ThiefAttack");
                            break;
                        case 2: // 여자 마법사
                            _anim.SetTrigger("MagicAttack");
                            break;
                        case 3: // 남자 궁수
                            _anim.SetTrigger("ArrowAttack");
                            CreateArrow();
                            break;
                        case 4: // 남자 도적
                            _anim.SetTrigger("ThiefAttack");
                            break;
                        case 5: // 남자 마법사
                            _anim.SetTrigger("MagicAttack");
                            break;
                        case 6: // 남자 전사
                            _anim.SetTrigger("WarriorAttack");
                            break;
                    }
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
    public void AttackTarget()
    {
        myTarget.GetComponent<IBattle>()?.OnDamage(myStat.AP);
    }

    public bool IsLive()
    {
        return Mathf.Approximately(myStat.HP, 0.0f);
    }

    public void FindTarget(Transform target)
    {
        myTarget = target;
        StopAllCoroutines();
        ChangeState(STATE.Battle);
    }

    public void LostTarget()
    {
        myTarget = null;
        StopAllCoroutines();
        _anim.SetBool("IsMoving", false);
        ChangeState(STATE.Idle);
    }

    public void AddAttacker(IBattle ib)
    {
        myAttackers.Add(ib);
    }

    public void DeadMessage(Transform tr)
    {
        if (tr == myTarget)
        {
            AttackSound.Stop();
            LostTarget();
            StartCoroutine(FinishQuest(2.0f));
        }
    }

    public void CreateArrow() // 화살 생성
    {
        GameObject obj = Instantiate(orgArrow, myBow.transform);
        obj.GetComponent<Arrow>().myTarget = myTarget;
        obj.GetComponent<Arrow>().OnFire();
    }
    public void MagicImpact()
    {
        Instantiate(magiceff, myTarget.position + myTarget.transform.up * 5.0f, Quaternion.identity);
    }
    public void FarmReturn()
    {
        SpawnManager.Instance.Teleport(gameObject);
        ChangeState(STATE.Moving);
        onSmile = false;
        Clockchk = false;
    }
    public void StateFarming()
    {
        ChangeState(STATE.Farming);
    }
}