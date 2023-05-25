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
    public void CreateBattleCard(Side _side, Position _pos, BattleCard.Kind _kind, bool IsSpy = false, bool isEnable = true)
    {
        // 指定位置のカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(_side, _pos);

        BoxCollider cardAreaCollider = cardArea.GetComponent<BoxCollider>();

        // 左上の手前
        Vector3 vecCardLeftTopUp = Common.GetBoxCollideVertices(cardAreaCollider)[0];
        // 左上の奥
        Vector3 vecCardLeftTopDown = Common.GetBoxCollideVertices(cardAreaCollider)[3];
        // カードエリアの厚み
        float cardAreaHeight = Vector3.Distance(vecCardLeftTopUp, vecCardLeftTopDown) / 2;

        // 生成位置
        Vector3 CreatePos = cardArea.gameObject.transform.position;

        // カードエリアの厚み分生成位置を上げる(カードエリアの上に生成するため)
        CreatePos.y += cardAreaHeight;

        GameObject cardClone = Instantiate(cardPrefab, CreatePos, Quaternion.identity);
        BattleCard battleCard = cardClone.GetComponent<BattleCard>();
        // 現在のターン側
        Side turnSide = BattleMgr.instance.GetTurnSide();
        // カードに設定する側
        Side cardSide = turnSide;

        // 値設定
        battleCard.SetSide(cardSide);
        battleCard.SetPosiiton(_pos);
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // スパイの場合の処理
        if (IsSpy)
        {
            // スパイの追加種類を設定
            battleCard.AddAppendKind(BattleCard.AppendKind.eAppendKind_Spy);
        }

        // 指定位置のカードエリアに登録
        cardArea.AddCard(battleCard);

        Debug.Log($"[{cardArea.GetSide()}]の'{cardArea.GetPosition()}'にカードを生成しました。");
    }
    public void CreateBattleCard(CardArea _cardArea, BattleCard.Kind _kind, bool IsSpy = false, bool isEnable = true)
    {
        // NULLチェック
        if(_cardArea == null) { return; }

        BoxCollider cardAreaCollider = _cardArea.GetComponent<BoxCollider>();
        // 左上の手前
        Vector3 vecCardLeftTopUp = Common.GetBoxCollideVertices(cardAreaCollider)[0];
        // 左上の奥
        Vector3 vecCardLeftTopDown = Common.GetBoxCollideVertices(cardAreaCollider)[3];
        // カードエリアの厚み
        float cardAreaHeight = Vector3.Distance(vecCardLeftTopUp, vecCardLeftTopDown) / 2;

        // 生成位置
        Vector3 CreatePos = _cardArea.gameObject.transform.position;

        // カードエリアの厚み分生成位置を上げる(カードエリアの上に生成するため)
        CreatePos.y += cardAreaHeight;

        GameObject cardClone = Instantiate(cardPrefab, CreatePos, Quaternion.identity);
        BattleCard battleCard = cardClone.GetComponent<BattleCard>();
        // 現在のターン
        Side turnSide = BattleMgr.instance.GetTurnSide();
        // カードに設定する側
        Side cardSide = turnSide;

        // 値設定
        battleCard.SetSide(cardSide);
        battleCard.SetPosiiton(_cardArea.GetPosition());
        battleCard.SetKind(_kind);
        battleCard.SetEnable(isEnable);

        // スパイの場合の処理
        if (IsSpy)
        {
            // スパイの追加種類を設定
            battleCard.AddAppendKind(BattleCard.AppendKind.eAppendKind_Spy);
        }

        // 指定位置のカードエリアに登録
        _cardArea.AddCard(battleCard);

        Debug.Log($"[{_cardArea.GetSide()}]の'{_cardArea.GetPosition()}'にカードを生成しました。");
    }

    // 指定カードの破棄
    public void RemoveBattleCard(BattleCard _battleCard)
    {
        Side cardSide = _battleCard.GetSide();
        Position cardPos = _battleCard.GetPosition();
        // 指定カードがいるカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(cardSide, cardPos);

        // カードエリアから情報を削除
        cardArea.RemoveCard(_battleCard);
        // ゲームオブジェクトがあれば
        if(_battleCard.gameObject != null)
        {
            // ゲームオブジェクトを削除
            Destroy(_battleCard.gameObject);
        }
    }
}
