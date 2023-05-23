using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;

public class BattleCardMgr : MonoBehaviour
{
    // インスタンス
    public static BattleCardMgr instance;

    // カードのPrehab
    public GameObject battleCardPrehab;

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
    // 指定種類カードを全て取得
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
    // 指定種類カード数を取得
    public int GetCardNumFromKind(Side _side, BattleCard.Kind _kind)
    {
        List<BattleCard> battleCardList = GetCardListFromKind(_side, _kind);
        return battleCardList.Count;
    }
}
