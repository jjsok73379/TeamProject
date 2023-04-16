using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Quest;
using static CharState;
using static VLNpc;
using System;

public class VLNpc : MonoBehaviour
{
    public NPCJOB job;
    public Quest.QuestInfo myQuest;
    public enum NPCJOB
    {
        COMMONS, NOBILLITY, ROYALTY
    }

    [Serializable]
    public struct VLNPC
    {
        [SerializeField] public string Name;
        [SerializeField] public NPCJOB NpcJob;
        [SerializeField] public Quest.QuestInfo myQuestInfo; // 생성될 때 명성에 따른 랜덤 퀘스트 분배
        [SerializeField] public int weight;

        public VLNPC(VLNPC vlnpc)
        {
            this.Name = vlnpc.Name;
            this.NpcJob = vlnpc.NpcJob;
            this.myQuestInfo = vlnpc.myQuestInfo;
            this.weight = vlnpc.weight;
        }
    }

    public static VLNPC RandomQuest(NPCJOB job)
    {
        //마을사람 F~D 귀족 C~B 왕족 A 퀘스트 스폰매니저랑 연동시키기
        //배열 세팅
        Quest.QuestGRADE[] C_RandomSet = new Quest.QuestGRADE[] { Quest.QuestGRADE.F, Quest.QuestGRADE.E, Quest.QuestGRADE.D };
        Quest.QuestGRADE[] N_RandomSet = new Quest.QuestGRADE[] { Quest.QuestGRADE.C, Quest.QuestGRADE.B };
        Quest.QuestGRADE[] R_RandomSet = new Quest.QuestGRADE[] { Quest.QuestGRADE.A };

        VLNPC Qnpc = new VLNPC();
        int n = UnityEngine.Random.Range(0, 2);

        Qnpc.NpcJob = job;
        switch (Qnpc.NpcJob) // 신분에 따라 케이스가 나뉘어짐
        {
            case VLNpc.NPCJOB.COMMONS:
                {
                    // F/E/D
                    switch (C_RandomSet[W_Random(C_RandomSet)])
                    {
                        case Quest.QuestGRADE.F:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "약초를 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "포션에 필요한 재료인 슬라임 10마리를 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.F;
                            Qnpc.myQuestInfo.rewardgold = 10;
                            Qnpc.myQuestInfo.rewardfame = 100;
                            break;
                        case Quest.QuestGRADE.E:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "마석 5개를 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "밭을 어지럽히는 고블린 10마리를 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.E;
                            Qnpc.myQuestInfo.rewardgold = 20;
                            Qnpc.myQuestInfo.rewardfame = 200;
                            break;
                        case Quest.QuestGRADE.D:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "가고일의 발톱을 3개 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "미궁에 들어가 가고일을 15마리를 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.D;
                            Qnpc.myQuestInfo.rewardgold = 30;
                            Qnpc.myQuestInfo.rewardfame = 300;
                            break;
                    }
                }
                Qnpc.Name = "평민";
                break;
            case VLNpc.NPCJOB.NOBILLITY:
                {
                    // B/C
                    switch (N_RandomSet[W_Random(N_RandomSet)])
                    {
                        case Quest.QuestGRADE.C:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "미노타우로스의 뿔을 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "사람을 해치는 오크 15마리를 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.C;
                            Qnpc.myQuestInfo.rewardgold = 50;
                            Qnpc.myQuestInfo.rewardfame = 500;
                            break;
                        case Quest.QuestGRADE.B:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "메탈골렘의 핵을 3개 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "미궁을 지키는 메탈골렘을 15마리를 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.B;
                            Qnpc.myQuestInfo.rewardgold = 100;
                            Qnpc.myQuestInfo.rewardfame = 1000;
                            break;
                    }
                }
                Qnpc.Name = "귀족";
                break;
            case VLNpc.NPCJOB.ROYALTY:
                {
                    //A
                    switch (R_RandomSet[W_Random(R_RandomSet)])
                    {
                        case Quest.QuestGRADE.A:
                            switch (n)
                            {
                                case 0:
                                    Qnpc.myQuestInfo.questname = "채집";
                                    Qnpc.myQuestInfo.information = "유니콘의 뿔을 1개 구해주세요!";
                                    break;
                                case 1:
                                    Qnpc.myQuestInfo.questname = "퇴치";
                                    Qnpc.myQuestInfo.information = "마을에 쳐들어온 드래곤을 퇴치해 주세요!";
                                    break;
                            }
                            Qnpc.myQuestInfo.questgrade = Quest.QuestGRADE.A;
                            Qnpc.myQuestInfo.rewardgold = 300;
                            Qnpc.myQuestInfo.rewardfame = 3000;
                            break;
                    }
                }
                Qnpc.Name = "왕족";
                break;
        }
        return Qnpc;
    }

    void Start()
    {
        myQuest = RandomQuest(job).myQuestInfo; // 랜덤퀘스트 함수의 myQuestInfo라는 변수를 마이퀘스트에 연동 -> 마이퀘스트 인포만 인스펙터에 보임
    }

    public void AddQuest() // 승낙
    {
        QuestManager.Instance.PostedQuest(myQuest);
    }

    public static int W_Random(Quest.QuestGRADE[] RandomSet) // 가중치 랜덤의 함수
    {
        int Lenght = RandomSet.Length;
        int fame = GameManager.Instance.Fame;
        int[] weights = new int[Lenght];

        switch (Lenght)
        {
            case 3:
                {
                    if (fame >= 0)
                    {
                        weights[0] = 100;
                        weights[1] = 0;

                        for (int i = 0; i < (fame / 1000); i++)
                        {
                            if (weights[0] != 0)
                            {
                                weights[0] -= (int)0.5f;
                                weights[1] += ((weights[1] >= 20) ? (int)0.4f : (int)0.5f);
                            }
                            else
                            {
                                if (weights[1] != 0)
                                {
                                    weights[1] -= (int)0.4f;
                                }
                                else if (weights[1] == 0)
                                {
                                    break;
                                }
                            }
                        }
                        weights[2] = 100 - (weights[0] + weights[1]);
                    }
                }
                break;
            case 2:
                {
                    if (fame >= 0)
                    {
                        weights[0] = 100;
                        for (int i = 0; i < (fame / 1000); i++)
                        {
                            if (weights[0] == 0)
                            {
                                break;
                            }
                            weights[0] -= (int)0.1f;
                        }
                        weights[1] = 100 - weights[0];
                    }
                }
                break;
            case 1:
                {
                    weights[0] = 100;
                }
                break;
        }
        // total이 100되게 설정
        int total = 0;
        for (int t = 0; t < weights.Length; ++t)
        { total += weights[t]; }

        // 랜덤포인트 설정
        int pivot = Mathf.RoundToInt(total * UnityEngine.Random.Range(0.0f, 1.0f));

        // 랜덤포인트가 해당하는 배열의 위치를 구별
        for (int n = 0; n < weights.Length; ++n)
        {
            if (pivot <= weights[n])
            {
                return n; // 배열의 [n]번째 값을 반환 
            }
            else
            {
                pivot -= weights[n];
            }
        }
        return 0; // 의미없음 어차피 b가 반환됨
        // 값을 반환하는 함수는 모든 코드 경로에 return문이 있어야함 / 없으면 오류
    }
}
