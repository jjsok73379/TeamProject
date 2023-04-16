using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static ADNpc;
using static CharState;

public class ADNpc : MonoBehaviour
{

    public GameObject AI_Per;

    public int adtype;

    public Sprite[] NpcProfiles;
    public CharState.NPC myStat;
    int Str, Dex, Int, Agi, bStr, bDex, bInt, bAgi, Atk, bAtk, Hel, bHel, mainStat;
    float AD, AR;

    //컴포넌트
    Host _host;
    public CharState.NPC RandomNPC(int a, Sprite[] np)
    {
        CharState.NPC adnpc = new CharState.NPC();
        adnpc.profile = np[a]; // 프로필 이미지는 변수 a에 따라 바뀌므로 해당 문장으로 통일
        switch (a)
        {
            case 0: // 여자 궁수
                adnpc.profile = np[0];
                adnpc.name = "셀리나";
                adnpc.npcJob = CharState.NPCJOB.ACHER;
                AR = 40.0f;
                AD = 1.0f;
                break;
            case 1: // 여자 도적
                adnpc.profile = np[1];
                adnpc.name = "사나";
                adnpc.npcJob = CharState.NPCJOB.THIEF;
                AR = 15.0f;
                AD = 2.0f;
                break;
            case 2: // 여자 마법사
                adnpc.profile = np[2];
                adnpc.name = "코리나";
                adnpc.npcJob = CharState.NPCJOB.WIZARD;
                AR = 30.0f;
                AD = 1.0f;
                break;
            case 3: // 남자 궁수
                adnpc.profile = np[3];
                adnpc.name = "브리오";
                adnpc.npcJob = CharState.NPCJOB.ACHER;
                AR = 40.0f;
                AD = 1.0f;
                break;
            case 4: // 남자 도적
                adnpc.profile = np[4];
                adnpc.name = "단테";
                adnpc.npcJob = CharState.NPCJOB.THIEF;
                AR = 15.0f;
                AD = 2.0f;
                break;
            case 5: // 남자 마법사
                adnpc.profile = np[5];
                adnpc.name = "클락";
                adnpc.npcJob = CharState.NPCJOB.WIZARD;
                AR = 30.0f;
                AD = 1.0f;
                break;
            case 6: // 남자 전사
                adnpc.profile = np[6];
                adnpc.name = "레오";
                adnpc.npcJob = CharState.NPCJOB.WARRIOR;
                AR = 15.0f;
                AD = 2.0f;
                break;
        }
        adnpc = Setstat(adnpc);
        return adnpc;
    }
    void Start()
    {
        NpcProfiles = Resources.LoadAll<Sprite>("Images/ProfileImages");
        myStat = RandomNPC(adtype, NpcProfiles); // 스타트에서 캐릭터에서 스탯이 주어짐
        _host = GetComponent<Host>();
        _host.myStat.RotSpeed = 360.0f;
        _host.myStat.MoveSpeed = 15.0f;
        _host.myStat.AttackRange = AR;
        _host.myStat.AttackDelay = AD;
        _host.myStat.AP = myStat.attack;
        _host.myStat.MaxHp = myStat.health;
        _host.myStat.HP = myStat.health;
        AI_Per.SetActive(false);
    }
    public CharState.NPC Setstat(CharState.NPC npc)
    {
        switch (npc.npcJob)
        {
            case CharState.NPCJOB.WARRIOR:
                Str = 10; Dex = 50; Int = 100; Agi = 20; //증가치 (작을수록 커짐)
                bStr = 40; bDex = 20; bInt = 10; bAgi = 30; //기본값
                Atk = 5; bAtk = 10; Hel = 5; bHel = 30;
                break;
            case CharState.NPCJOB.ACHER:
                Str = 100; Dex = 10; Int = 50; Agi = 20;
                bStr = 10; bDex = 40; bInt = 20; bAgi = 30;
                Atk = 5; bAtk = 20; Hel = 5; bHel = 15;
                break;
            case CharState.NPCJOB.THIEF:
                Str = 50; Dex = 20; Int = 100; Agi = 10;
                bStr = 40; bDex = 20; bInt = 10; bAgi = 30;
                Atk = 5; bAtk = 20; Hel = 5; bHel = 20;
                break;
            case CharState.NPCJOB.WIZARD:
                Str = 100; Dex = 20; Int = 10; Agi = 50;
                bStr = 10; bDex = 30; bInt = 40; bAgi = 20;
                Atk = 5; bAtk = 30; Hel = 5; bHel = 10;
                break;
        }

        npc.strong = bStr + Mathf.Clamp(UnityEngine.Random.Range(0, (GameManager.Instance.Fame / Str)), 0, 99999999); // 10당 1씩 증가
        npc.dexterity = bDex + Mathf.Clamp(UnityEngine.Random.Range(0, (GameManager.Instance.Fame / Dex)), 0, 99999999); // 10당 10씩 증가
        npc.intellect = bInt + Mathf.Clamp(UnityEngine.Random.Range(0, (GameManager.Instance.Fame / Int)), 0, 99999999); // 10당 1씩 증가
        npc.agility = bAgi + Mathf.Clamp(UnityEngine.Random.Range(0, (GameManager.Instance.Fame / Agi)), 0, 99999999); // 10당 10씩증가

        switch (npc.npcJob)
        {
            case CharState.NPCJOB.WARRIOR:
                mainStat = npc.strong;
                break;
            case CharState.NPCJOB.ACHER:
                mainStat = npc.dexterity;
                break;
            case CharState.NPCJOB.WIZARD:
                mainStat = npc.intellect;
                break;
            case CharState.NPCJOB.THIEF:
                mainStat = npc.agility;
                break;
        }

        npc.health = bHel + (mainStat / Hel);

        npc.attack = bAtk + (mainStat / Atk);
        npc.charGrade = Grade(mainStat);
        return npc;
    }

    public CharState.GRADE Grade(int main)
    {
        CharState.GRADE grade = new CharState.GRADE();
        if (main <= 140) // 기본스텟 40
        {
            grade = CharState.GRADE.F;
        }
        else if (main <= 640 && main > 140)
        {
            grade = CharState.GRADE.E;
        }
        else if (main <= 1640 && main > 640)
        {
            grade = CharState.GRADE.D;
        }
        else if (main <= 4640 && main > 1640)
        {
            grade = CharState.GRADE.C;
        }
        else if (main <= 9640 && main > 4640)
        {
            grade = CharState.GRADE.B;
        }
        else if (main > 9640)
        {
            grade = CharState.GRADE.A;
        }
        return grade;
    }
}


