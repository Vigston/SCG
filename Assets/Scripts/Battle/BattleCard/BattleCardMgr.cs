using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;

public class BattleCardMgr : MonoBehaviour
{
    // インスタンス
    public static BattleCardMgr instance;

    // カードリスト
    [SerializeField]
    private List<BattleCard> m_BattleCardList;

    // カードのPrehab
    public GameObject battleCardPrehab;

    // 科学カードマテリアル
    public Material m_ScienceMaterial;
    // 軍事カードマテリアル
    public Material m_MilitaryMaterial;
    // スパイカードマテリアル
    public Material m_SpyMaterial;

    private void Awake()
    {
        CreateInstance();
    }

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

        Debug.LogError("BattleCardMgrのインスタンスが生成できませんでした");
        return false;
    }

    // ---関数---
    // カードリストのコピー
    public void CopyCardList(List<BattleCard> _battleCardList)
    {
        m_BattleCardList = new List<BattleCard>(_battleCardList);
    }
    // カードリストの取得
    public List<BattleCard> GetCardList()
    {
        return m_BattleCardList;
    }
    // カード追加
    public void AddCard(BattleCard _battleCard)
    {
        m_BattleCardList.Add(_battleCard);
    }
    // カード削除
    public void RemoveCard(BattleCard _battleCard)
    {
        m_BattleCardList.Remove(_battleCard);
    }

    // 指定基本種類カードを全て取得
    public List<BattleCard> GetCardListFromKind(Side _side, BattleCard.Kind _kind)
    {
        List<BattleCard> searchCardList = new List<BattleCard>();
        for(int i = 0; i < (int)Position.ePosition_Max; i++)
        {
            Position pos = (Position)i;

            // カードエリア
            CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(_side, pos);

            List<BattleCard> areaCardList = cardArea.GetCardList();

            // エリアにカードがないなら次のエリア検索へ
            if (!areaCardList.Any()) { continue; }

            // カード条件追加
            foreach(BattleCard areaCard in areaCardList)
            {
                // 指定種類じゃないならはじく
                if(areaCard.GetKind() != _kind) { continue; }

                // 追加
                searchCardList.Add(areaCard);
            }
        }

        return searchCardList;
    }
    // 指定追加種類カードを全て取得
    public List<BattleCard> GetCardListFromAppendKind(Side _side, BattleCard.AppendKind _appendKind)
    {
        List<BattleCard> searchCardList = new List<BattleCard>();
        for (int i = 0; i < (int)Position.ePosition_Max; i++)
        {
            Position pos = (Position)i;

            // カードエリア
            CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(_side, pos);

            List<BattleCard> areaCardList = cardArea.GetCardList();

            // エリアにカードがないなら次のエリア検索へ
            if (!areaCardList.Any()) { continue; }

            // カード条件追加
            foreach (BattleCard areaCard in areaCardList)
            {
                // 指定追加種類を持っていないならはじく
                if (!areaCard.IsHaveAppendKind(_appendKind)) { continue; }

                // 追加
                searchCardList.Add(areaCard);
            }
        }

        return searchCardList;
    }
    // 指定基本種類カード数を取得
    public int GetCardNumFromKind(Side _side, BattleCard.Kind _kind)
    {
        List<BattleCard> battleCardList = GetCardListFromKind(_side, _kind);
        return battleCardList.Count;
    }
    // 指定追加種類カード数を取得
    public int GetCardNumFromAppendKind(Side _side, BattleCard.AppendKind _appendKind)
    {
        List<BattleCard> battleCardList = GetCardListFromAppendKind(_side, _appendKind);

        return battleCardList.Count;
    }
}
