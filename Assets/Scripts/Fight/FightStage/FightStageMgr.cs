using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FightStageMgr : MonoBehaviour
{
    // =================変数=================
    [SerializeField]
    private List<Area> areaList = new List<Area>();

    // =================構造体=================
    [System.Serializable]
    private class Area
    {
        // インデックス
        public int index;
        // 名前
        public string name;
    }

    // Start is called before the first frame update
    void Start()
    {
        // エリアリストの初期化
        InitAreaList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =================関数=================
    // エリアリストの初期化
    void InitAreaList()
    {
        areaList = new List<Area>();
    }

    // エリアの追加
    public void AddArea(int _index, string _name)
    {
        Area area = new Area();
        // 値設定
        area.index = _index;
        area.name = _name;
        // 追加
        areaList.Add(area);
    }

    // 指定エリアの削除
    public void RemoveArea(int index)
    {
        Area area = new Area();
        // 指定Indexのエリアを取得
        area = areaList[index];
        // 削除
        areaList.Remove(area);
    }

    // 全てのエリアを削除
    public void AllRemoveArea()
    {
        // 全て削除
        areaList.Clear();
    }
}
