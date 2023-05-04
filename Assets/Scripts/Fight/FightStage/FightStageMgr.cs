using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FightStageMgr : MonoBehaviour
{
    // =================変数=================
    public static FightStageMgr instance;

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

    void Awake()
    {
        // インスタンスを作成
        CreateInstance();
        // エリアリストの初期化
        InitAreaList();
    }

    // Start is called before the first frame update
    void Start()
    {
        // カードエリアの作成
        FightStageCtr.instance.CreateCardArea();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // =================関数=================
    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if (instance == null)
        {
            // 作成
            instance = this;
            return true;
        }

        Debug.LogError("FightStageMgrのインスタンスが生成できませんでした");
        return false;
    }

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
