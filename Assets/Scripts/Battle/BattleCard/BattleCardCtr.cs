using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCardCtr : MonoBehaviour
{
    // インスタンス
    public static BattleCardCtr instance;

    // カードオブジェクト
    public GameObject cardPrefab;

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

    // 指定位置にカードを生成
    public void CreateBattleCard(Side _side,Position _pos, BattleCard.Kind _kind, bool isEnable = true)
    {
        // 指定位置のカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(_side, _pos);

        GameObject cardClone = Instantiate(cardPrefab, cardArea.gameObject.transform.position, Quaternion.identity);
        BattleCard battleCard = cardClone.GetComponent<BattleCard>();
        // 現在のターン
        Side turnSide = BattleMgr.instance.GetTurnSide();
        // 値設定
        battleCard.SetSide(turnSide);
        battleCard.SetPosiiton(_pos);
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // 指定位置のカードエリアに登録
        cardArea.AddCard(battleCard);

        Debug.Log($"[{cardArea.GetSide()}]の'{cardArea.GetPosition()}'にカードを生成しました。");
    }
    public void CreateBattleCard(CardArea _cardArea, BattleCard.Kind _kind, bool isEnable = true)
    {
        // NULLチェック
        if(_cardArea == null) { return; }

        GameObject cardClone = Instantiate(cardPrefab, _cardArea.gameObject.transform.position, Quaternion.identity);
        BattleCard battleCard = cardClone.GetComponent<BattleCard>();
        // 現在のターン
        Side turnSide = BattleMgr.instance.GetTurnSide();
        // 値設定
        battleCard.SetSide(turnSide);
        battleCard.SetPosiiton(_cardArea.GetPosition());
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // 指定位置のカードエリアに登録
        _cardArea.AddCard(battleCard);

        Debug.Log($"[{_cardArea.GetSide()}]の'{_cardArea.GetPosition()}'にカードを生成しました。");
    }
}
