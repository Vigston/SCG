using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCardCtr : MonoBehaviour
{
    // インスタンス
    public static BattleCardCtr instance;

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

        Debug.LogError("BattleCardCtrのインスタンスが生成できませんでした");
        return false;
    }

    public void CreateBattleCard(Position _pos, BattleCard.Kind _kind, bool isEnable = true)
    {
        BattleCard battleCard = new BattleCard();
        // 値設定
        battleCard.SetPosiiton(_pos);
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // 指定位置のカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetSearchCardArea(_pos);

        // 指定位置のカードエリアにカードを移動
        cardArea.AddCard(battleCard);
    }
}
