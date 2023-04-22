using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FightStageMgr : MonoBehaviour
{
    // =================�ϐ�=================
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

    // Start is called before the first frame update
    void Start()
    {
        // �G���A���X�g�̏�����
        InitAreaList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =================�֐�=================
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
