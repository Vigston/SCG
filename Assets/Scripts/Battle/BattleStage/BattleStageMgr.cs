using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using battleTypes;
using System.Linq;

public class BattleStageMgr : MonoBehaviour
{
    // =================変数=================
    public static BattleStageMgr instance;

    // プレイヤーエリア
    public BoxCollider PlayerArea;
    // エネミーエリア
    public BoxCollider EnemyArea;

    [SerializeField]
    private List<CardArea> m_CardAreaList = new List<CardArea>();

    // =================構造体=================

    void Awake()
    {
        // インスタンスを作成
        CreateInstance();
        // カードエリアリストの初期化
        InitCardAreaList();
    }

    // Start is called before the first frame update
    void Start()
    {
        // カードエリアの作成
        BattleStageCtr.instance.CreateCardArea();
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

    // ===エリア===
    // 自分のエリア

    // ===カードエリア===
    // カードエリアリストの初期化
    void InitCardAreaList()
    {
        m_CardAreaList = new List<CardArea>();
    }

    // カードエリアリストのコピー
    public void CopyCardAreaList(List<CardArea> areaList)
    {
        m_CardAreaList = areaList;
    }

    // カードエリアの追加
    public void AddCardArea(CardArea _cardArea)
    {
        // 追加
        m_CardAreaList.Add(_cardArea);
    }
    public void AddCardArea(Position _pos)
    {
        CardArea cardArea = new CardArea();
        // 値設定
        cardArea.SetPosiiton(_pos);
        // 追加
        m_CardAreaList.Add(cardArea);
    }


    // 指定カードエリアの削除
    public void RemoveCardArea(int index)
    {
        CardArea cardArea = new CardArea();
        // 指定Indexのエリアを取得
        cardArea = m_CardAreaList[index];
        // 削除
        m_CardAreaList.Remove(cardArea);
    }

    // 全てのカードエリアを削除
    public void AllRemoveCardArea()
    {
        // 全て削除
        m_CardAreaList.Clear();
    }

    // 指定側のカードエリアリストを取得
    public List<CardArea> GetCardAreaListFromSide(Side _side)
    {
        // 指定側のカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach(CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if(cardArea.GetSide() != _side) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // カードが入ってないカードエリアリストを取得
    public List<CardArea> GetCardAreaListFromEmptyCard()
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // カードが入っているならはじく
            if (!cardArea.IsCardEmpty()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // カードが入ってないカードエリアリスト数を取得
    public int GetCardAreaNumFromEmptyCard()
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // カードが入っているならはじく
            if (!cardArea.IsCardEmpty()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList.Count;
    }

    // 指定側のカードが入っていないカードリストエリアを取得
    public List<CardArea> GetCardAreaFromSideEmptyCard(Side _side)
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if (cardArea.GetSide() != _side) { continue; }
            // カードが入っているならはじく
            if (!cardArea.IsCardEmpty()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // 指定位置のカードエリアを取得
    public CardArea GetCardAreaFromPos(Side _side, Position _pos)
    {
        CardArea cardArea = m_CardAreaList.Find(x => x.GetSide() == _side && x.GetPosition() == _pos);

        return cardArea;
    }
}
