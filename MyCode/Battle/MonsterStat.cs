using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public GRADE grade;

    public enum GRADE
    {
        F, E, D, C, B, A
    }

    public CharacterStat myStat;

    public struct MONSTERSTAT
    {
        public CharacterStat myStatInfo;
    }

    public static MONSTERSTAT RandomMonster(GRADE g)
    {
        MONSTERSTAT monster = new MONSTERSTAT();
        switch (g)
        {
            case GRADE.F:
                monster.myStatInfo.MaxHp = 50.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 5.0f;
                break;
            case GRADE.E:
                monster.myStatInfo.MaxHp = 100.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 10.0f;
                break;
            case GRADE.D:
                monster.myStatInfo.MaxHp = 200.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 20.0f;
                break;
            case GRADE.C:
                monster.myStatInfo.MaxHp = 400.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 40.0f;
                break;
            case GRADE.B:
                monster.myStatInfo.MaxHp = 800.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 80.0f;
                break;
            case GRADE.A:
                monster.myStatInfo.MaxHp = 1600.0f;
                monster.myStatInfo.HP = monster.myStatInfo.MaxHp;
                monster.myStatInfo.AP = 160.0f;
                break;
        }
        return monster;
    }
    // Start is called before the first frame update
    void Start()
    {
        myStat = RandomMonster(grade).myStatInfo;
        GetComponent<Monster>().myStat.MaxHp = myStat.MaxHp;
        GetComponent<Monster>().myStat.HP = myStat.MaxHp;
        GetComponent<Monster>().myStat.AP = myStat.AP;
    }
}
