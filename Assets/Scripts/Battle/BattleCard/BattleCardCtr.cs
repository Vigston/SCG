using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCardCtr : MonoBehaviour
{
    // �C���X�^���X
    public static BattleCardCtr instance;

    private void Awake()
    {
        CreateInstance();
    }

    // �C���X�^���X���쐬
    public bool CreateInstance()
    {
        // ���ɃC���X�^���X���쐬����Ă��Ȃ���΍쐬����
        if (instance == null)
        {
            // �쐬
            instance = this;
        }

        // �C���X�^���X���쐬�ς݂Ȃ�I��
        if (instance != null) { return true; }

        Debug.LogError("BattleCardCtr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    public void CreateBattleCard(Position _pos, BattleCard.Kind _kind, bool isEnable = true)
    {
        BattleCard battleCard = new BattleCard();
        // �l�ݒ�
        battleCard.SetPosiiton(_pos);
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // �w��ʒu�̃J�[�h�G���A
        CardArea cardArea = BattleStageMgr.instance.GetSearchCardArea(_pos);

        // �w��ʒu�̃J�[�h�G���A�ɃJ�[�h���ړ�
        cardArea.AddCard(battleCard);
    }
}
