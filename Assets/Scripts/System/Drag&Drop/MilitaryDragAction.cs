using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using battleTypes;
using Photon.Pun;
using static Common;

public class MilitaryDragAction : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject m_ActionObj;
    [SerializeField]
    private GameObject m_TargetCard;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 軍事行動が行えないならはじく
        if (!IsMilitalyAction()) { return; }
        m_ActionObj = gameObject;
        m_TargetCard = null;
        Debug.Log($"{m_ActionObj}を軍事行動ドラッグしました。");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 軍事行動が行えないならはじく
        if (!IsMilitalyAction()) { return; }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 軍事行動が行えないならはじく
        if (!IsMilitalyAction()) { return; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.collider.tag == "BattleCard")
            {
                // 自分以外ならターゲットに設定する
                if(hit.transform.gameObject != gameObject)
                {
                    m_TargetCard = hit.transform.gameObject;
                }
            }
        }

        // ターゲットカードオブジェクトが取得できていないならはじく
        if (!m_TargetCard) return;

		BattleCard targetCard = m_TargetCard.GetComponent<BattleCard>();
        if (!targetCard) return;

		// 対象カードが自分のカードならはじく
		if (IsMyCard(targetCard)) return;

		BattleCard actionCard = m_ActionObj.GetComponent<BattleCard>();
        if (!actionCard) return;

		// 対象カードを破壊
		BattleCardCtr.instance.RemoveBattleCard(targetCard);

		// 対象カードが軍事カードなら自分も破壊
		if (targetCard.IsHaveAppendKind(BattleCard.JobKind.eAppendKind_Military))
		{
			// 自分を破壊
			BattleCardCtr.instance.RemoveBattleCard(actionCard);
		}

		Military military = actionCard.GetMilitary();
		// アクションが終わったので非武装にする
		military.SetStatus(Military.Status.eStatus_Unarmed);
		actionCard.SetMaterial(BattleCard.JobKind.eAppendKind_Military);

		// カードの行動回数加算
		actionCard.AddActionNum();

        NetWorkSync.instance.photonView.RPC(nameof(NetWorkSync.instance.RPC_MilitaryAction), RpcTarget.Others, (int)GetRevSide(actionCard.GetSetSide), (int)actionCard.GetSetPosition, (int)GetRevSide(targetCard.GetSetSide), (int)targetCard.GetSetPosition);

		m_ActionObj = null;
        Debug.Log($"軍事行動ドラッグ終了。");
    }

    // 軍事行動ができるか
    public bool IsMilitalyAction()
    {
        BattleCard battleCard = gameObject.GetComponent<BattleCard>();
		// バトルカードじゃないならはじく
		if (battleCard == null) { return false; }
        // 疲労状態ならはじく
        if (battleCard.IsStatus(BattleCard.Status.eStatus_Fatigue)) { return false; }
		// 行動できないならはじく
		if (!battleCard.IsAction()) { return false; }
		// 自分のターンで自分のカードじゃないならはじく
		if (!IsMyTurnAndMyCard(battleCard)) { return false; }
		// メインフェイズじゃなければはじく
		if (!BattleMgr.instance.IsPhase(PhaseType.ePhaseType_Main)) { return false; }

		Military military = battleCard.GetMilitary();
        // 軍事じゃないならはじく
        if(military == null) { return false; }
		// 非武装状態ならはじく
		if (military.IsStatus(Military.Status.eStatus_Unarmed))
        {
            Debug.Log("非武装状態なので行動出来ません！！");
            return false;
        }

		// 行動できる
		return true;
    }
}
