using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using battleTypes;

public class BattleStageMgr : MonoBehaviour
{
    // =================変数=================
    public static BattleStageMgr instance;

    [SerializeField]
    private List<CardArea> m_CardAreaList = new List<CardArea>();

    // =================構造体=================

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
        BattleStageCtr.instance.CreateCardArea(m_CardAreaList);
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
        }

        // インスタンスが作成済みなら終了
        if (instance != null) { return true; }

        Debug.LogError("BattleStageMgrのインスタンスが生成できませんでした");
        return false;
    }

    // エリアリストの初期化
    void InitAreaList()
    {
        m_CardAreaList = new List<CardArea>();
    }

    // エリアの追加
    public void AddArea(Position _pos)
    {
        CardArea cardArea = new CardArea();
        // 値設定
        cardArea.SetPosiiton(_pos);
        // 追加
        m_CardAreaList.Add(cardArea);
    }

    // 指定エリアの削除
    public void RemoveArea(int index)
    {
        CardArea cardArea = new CardArea();
        // 指定Indexのエリアを取得
        cardArea = m_CardAreaList[index];
        // 削除
        m_CardAreaList.Remove(cardArea);
    }

    // 全てのエリアを削除
    public void AllRemoveArea()
    {
        // 全て削除
        m_CardAreaList.Clear();
    }

    // カードエリアを検索して取得
    public CardArea GetSearchCardArea(Position _pos)
    {
        CardArea cardArea = m_CardAreaList.Find(x => x.GetPosition() == _pos);

        return cardArea;
    }

    // カードエリアを検索してTransformを取得
    public Transform GetTransformSearchCardArea(Position _pos)
    {
        CardArea cardArea = m_CardAreaList.Find(x => x.GetPosition() == _pos);

        Transform transform = cardArea.gameObject.transform;

        return transform;
    }
}
