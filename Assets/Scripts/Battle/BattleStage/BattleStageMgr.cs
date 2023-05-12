using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using battleTypes;

public class BattleStageMgr : MonoBehaviour
{
    // =================�ϐ�=================
    public static BattleStageMgr instance;

    [SerializeField]
    private List<CardArea> m_CardAreaList = new List<CardArea>();

    // =================�\����=================

    void Awake()
    {
        // �C���X�^���X���쐬
        CreateInstance();
        // �G���A���X�g�̏�����
        InitAreaList();
    }

    // Start is called before the first frame update
    void Start()
    {
        // �J�[�h�G���A�̍쐬
        BattleStageCtr.instance.CreateCardArea(m_CardAreaList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =================�֐�=================
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

        Debug.LogError("BattleStageMgr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    // �G���A���X�g�̏�����
    void InitAreaList()
    {
        m_CardAreaList = new List<CardArea>();
    }

    // �G���A�̒ǉ�
    public void AddArea(Position _pos)
    {
        CardArea cardArea = new CardArea();
        // �l�ݒ�
        cardArea.SetPosiiton(_pos);
        // �ǉ�
        m_CardAreaList.Add(cardArea);
    }

    // �w��G���A�̍폜
    public void RemoveArea(int index)
    {
        CardArea cardArea = new CardArea();
        // �w��Index�̃G���A���擾
        cardArea = m_CardAreaList[index];
        // �폜
        m_CardAreaList.Remove(cardArea);
    }

    // �S�ẴG���A���폜
    public void AllRemoveArea()
    {
        // �S�č폜
        m_CardAreaList.Clear();
    }

    // �J�[�h�G���A���������Ď擾
    public CardArea GetSearchCardArea(Position _pos)
    {
        CardArea cardArea = m_CardAreaList.Find(x => x.GetPosition() == _pos);

        return cardArea;
    }

    // �J�[�h�G���A����������Transform���擾
    public Transform GetTransformSearchCardArea(Position _pos)
    {
        CardArea cardArea = m_CardAreaList.Find(x => x.GetPosition() == _pos);

        Transform transform = cardArea.gameObject.transform;

        return transform;
    }
}
