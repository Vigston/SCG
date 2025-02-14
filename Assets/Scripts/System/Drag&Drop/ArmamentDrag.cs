using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using battleTypes;
using Photon.Pun;
using static Common;

public class ArmamentDrag : MonoBehaviourPun, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private const int costGoldValue = 20;
    [SerializeField]
    private GameObject targetCard;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 武装付与が行えるなら
        if(IsArmament())
        {
            targetCard = null;
            Debug.Log("武装付与ドラッグしました。");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 武装付与が行えるなら
        if (IsArmament())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                // クリックしたオブジェクト
                GameObject hitObj = hit.transform.gameObject;

                if (hit.collider.tag == "BattleCard")
                {
                    targetCard = hitObj;
                }
            }

            // カードにドロップしているなら
            if (targetCard != null)
            {
                // カード種類を設定
                BattleCard battleCard = targetCard.GetComponent<BattleCard>();

                // バトルカードじゃないならはじく
                if(battleCard == null) { return; }

                // 軍事カードじゃないならはじく。
                if(!battleCard.IsAppendJob(BattleCard.JobKind.eAppendKind_Military))
                {
                    Debug.Log("軍事カードじゃないので武装付与をキャンセルします。");

                    return;
                }

                // 自分のターンで自分のカードなら武装付与できる
                if (Common.IsMyTurnAndMyCard(battleCard))
                {
                    Military military = battleCard.GetMilitary();
                    Side turnSide = BattleMgr.instance.GetSetTurnSide;

                    // 軍事カードじゃないならはじく
                    if(military == null) { return; }

                    Military.Status     militaryStatus  = Military.Status.eStatus_Armed;
                    BattleCard.JobKind jobKind = BattleCard.JobKind.eAppendKind_Military;

					// 武装付与
					military.SetStatus(militaryStatus);
                    battleCard.SetMaterial(jobKind);
                    // コスト分Goldを減らす
                    BattleMgr.instance.ReduceGoldValue(turnSide, costGoldValue);

					photonView.RPC(nameof(RPC_Armament), RpcTarget.Others, (int)militaryStatus, (int)jobKind, (int)GetRevSide(battleCard.GetSetSide), (int)battleCard.GetSetPosition);
                }
            }

            Debug.Log("武装付与ドラッグ終了。");
        }
    }

    // 武装付与が行えるか
    public bool IsArmament()
    {
        // 操作側
        Side operateSide = BattleUserMgr.instance.GetSetOperateSide;
        // ターン側
        Side turnSide = BattleMgr.instance.GetSetTurnSide;
        // ゴールド数
        int GoldValue = BattleMgr.instance.GetGoldValue(turnSide);

        // デバッグモード中じゃないなら
        if(!DebugMgr.instance.IsDebugMode())
        {
			// 操作側が自分じゃないならはじく
			if (operateSide != Side.eSide_Player) { return false; }
			// 自分のターンじゃなければはじく
			if (!Common.IsMyTurn()) { return false; }
		}
		// メインフェイズじゃなければはじく
		if (!BattleMgr.instance.IsPhase(PhaseType.ePhaseType_Main)) { return false; }
		// 場に軍事カードがいなければはじく
		if (BattleCardMgr.instance.GetCardNumFromAppendKind(turnSide, BattleCard.JobKind.eAppendKind_Military) <= 0) { return false; }
		// 武装に必要なゴールドが足らないならはじく
		if (GoldValue < costGoldValue) { return false; }


		// 行える
		return true;
    }

    // 武装付与
    [PunRPC]
    void RPC_Armament(int _militaryStatus, int _jobKind, int _cardSide, int _cardPos)
    {
        // カードエリア
        CardArea cardArea = BattleStageMgr.instance.GetCardAreaFromPos((Side)_cardSide, (Position)_cardPos);
        if (!cardArea) return;
        // カード
        BattleCard card = cardArea.GetCard(0);
        if (!card) return;
        // 軍事
        Military military = card.GetMilitary();
        if (!military) return;

        // ターン側
		Side turnSide = BattleMgr.instance.GetSetTurnSide;

		// 武装付与
		military.SetStatus((Military.Status)_militaryStatus);
		card.SetMaterial((BattleCard.JobKind)_jobKind);
		// コスト分Goldを減らす
		BattleMgr.instance.ReduceGoldValue(turnSide, costGoldValue);
	}
}
