using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FightStageMgr : MonoBehaviour
{
    // =================�ϐ�=================
    public static FightStageMgr instance;

    [SerializeField]
    private List<Area> areaList = new List<Area>();

    // =================�\����=================
    [System.Serializable]
    private class Area
    {
        // �C���f�b�N�X
        public int index;
        // ���O
        public string name;
    }

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
        FightStageCtr.instance.CreateCardArea();
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
            return true;
        }

        Debug.LogError("FightStageMgr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    // �G���A���X�g�̏�����
    void InitAreaList()
    {
        areaList = new List<Area>();
    }

    // �G���A�̒ǉ�
    public void AddArea(int _index, string _name)
    {
        Area area = new Area();
        // �l�ݒ�
        area.index = _index;
        area.name = _name;
        // �ǉ�
        areaList.Add(area);
    }

    // �w��G���A�̍폜
    public void RemoveArea(int index)
    {
        Area area = new Area();
        // �w��Index�̃G���A���擾
        area = areaList[index];
        // �폜
        areaList.Remove(area);
    }

    // �S�ẴG���A���폜
    public void AllRemoveArea()
    {
        // �S�č폜
        areaList.Clear();
    }
}
