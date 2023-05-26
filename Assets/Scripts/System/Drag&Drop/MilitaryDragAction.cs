using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using battleTypes;

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

        // ターゲットがいる場合
        if(m_TargetCard != null)
        {
            // アクションを行ったか
            bool IsAction = false;
            BattleCard targetCard = m_TargetCard.GetComponent<BattleCard>();
            // ターゲットがカードの場合
            if(targetCard != null)
            {
                Side turnSide = BattleMgr.instance.GetTurnSide();
                // 対象カードがターン側
                if (targetCard.GetSide() == turnSide)
                {
                    // 対象カードがスパイなら
                    if(targetCard.IsHaveAppendKind(BattleCard.AppendKind.eAppendKind_Spy))
                    {
                        // 対象カードを破壊
                        BattleCardCtr.instance.RemoveBattleCard(targetCard);
                    }

                    IsAction = true;
                }
                // 対象カードがターンの逆側
                else if(targetCard.GetSide() != turnSide)
                {
                    BattleCard actionCard = m_ActionObj.GetComponent<BattleCard>();
                    // 自分がスパイじゃないなら
                    if(!actionCard.IsHaveAppendKind(BattleCard.AppendKind.eAppendKind_Spy))
                    {
                        // 対象カードを破壊
                        BattleCardCtr.instance.RemoveBattleCard(targetCard);
                        // 対象カードが軍事カードなら自分も破壊
                        if (targetCard.IsHaveAppendKind(BattleCard.AppendKind.eAppendKind_Military))
                        {
                            // 自分を破壊
                            BattleCardCtr.instance.RemoveBattleCard(actionCard);
                        }
                    }

                    IsAction = true;
                }

                // アクションを行っているなら
                if(IsAction)
                {
                    BattleCard actionCard = m_ActionObj.GetComponent<BattleCard>();
                    if(actionCard != null)
                    {
                        // カードの行動回数加算
                        actionCard.AddActionNum();
                    }
                    // BattleMgr更新リクエスト
                    BattleMgr.instance.UpdateRequest();
                }
            }
        }

        m_ActionObj = null;
        Debug.Log($"軍事行動ドラッグ終了。");
    }

    // 軍事行動ができるか
    public bool IsMilitalyAction()
    {
        BattleCard battleCard = gameObject.GetComponent<BattleCard>();
        // バトルカードじゃないならはじく
        if (battleCard == null) { return false; }
        // このターンに登場したならはじく
        if (battleCard.IsEntryThisTurn()) { return false; }
        // 行動できないならはじく
        if (!battleCard.IsAction()) { return false; }

        // 行動できる
        return true;
    }
}
