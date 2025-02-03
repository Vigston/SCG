using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Photon.Pun;
using static Common;

public class BattleCardCtr : MonoBehaviourPun
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
    public void CreateBattleCard(Side _Side, Position _pos, BattleCard.Kind _kind, bool IsSpy = false, bool isEnable = true)
    {
        // 指定位置のカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(_Side, _pos);

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

		GameObject cardClone = Instantiate(cardPrefab, CreatePos, Quaternion.Euler(0f, 180f, 0f));
		BattleCard battleCard = cardClone.GetComponent<BattleCard>();

        // 現在のターン側
        Side turnSide = BattleMgr.instance.GetSetTurnSide;
        // カードに設定する側
        Side cardSide = turnSide;

        // 値設定
        battleCard.GetSetSide       =   cardSide;
        battleCard.GetSetPosition   =   _pos;
        battleCard.GetSetStatus     =   BattleCard.Status.eStatus_Fatigue;
        battleCard.GetSetKind       =   _kind;
        battleCard.SetEnable(isEnable);
        battleCard.SetEntryTurn();

        // スパイの場合の処理
        if (IsSpy)
        {
            // スパイ職業を付与
            battleCard.AppendJob(BattleCard.JobKind.eAppendKind_Spy);
        }

        // BattleCardMgrに登録
        BattleCardMgr.instance.AddCard(battleCard);
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

        GameObject cardClone = PhotonNetwork.Instantiate(PrefabPath.Card, CreatePos, Quaternion.Euler(0f, 180f, 0f));
		BattleCard battleCard = cardClone.GetComponent<BattleCard>();

        int cardViewId = battleCard.photonView.ViewID;
		// 現在のターン
		Side turnSide = BattleMgr.instance.GetSetTurnSide;
		// カードに設定する側
		Side cardSide = turnSide;

        photonView.RPC(nameof(CreateBattleCard_RPC), RpcTarget.All, cardViewId, (int)cardSide, (int)_cardArea.GetPosition(), (int)BattleCard.Status.eStatus_Fatigue, (int)_kind, isEnable, IsSpy);
    }
    [PunRPC]
    void CreateBattleCard_RPC(int _battleCardViewId, int _side, int _areaPos, int _status, int _kind, bool isEnable, bool _isSpy)
    {
        BattleCard battleCard = PhotonView.Find(_battleCardViewId)?.gameObject.GetComponent<BattleCard>();
        if (battleCard == null) return;

		// 値設定
		battleCard.GetSetSide = (Side)_side;
		battleCard.GetSetPosition = (Position)_areaPos;
		battleCard.GetSetStatus = (BattleCard.Status)_status;
		battleCard.GetSetKind = (BattleCard.Kind)_kind;
		battleCard.SetEnable(isEnable);
		battleCard.SetEntryTurn();

		// スパイの場合の処理
		if (_isSpy)
		{
			// スパイ職業を付与
			battleCard.AppendJob(BattleCard.JobKind.eAppendKind_Spy);
		}

		// BattleCardMgrに登録
		BattleCardMgr.instance.AddCard(battleCard);

        // 指定位置のカードエリアに登録
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos((Side)_side, (Position)_areaPos);
		cardArea.AddCard(battleCard);

		Debug.Log($"[{cardArea.GetSide()}]の'{cardArea.GetPosition()}'にカードを生成しました。");
	}

    // 指定カードの破棄
    public void RemoveBattleCard(BattleCard _battleCard)
    {
        Side cardSide     = _battleCard.GetSetSide;
        Position cardPos  = _battleCard.GetSetPosition;
        // 指定カードがいるカードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos(cardSide, cardPos);

        // BattleCardMgrから情報を削除
        BattleCardMgr.instance.RemoveCard(_battleCard);
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
