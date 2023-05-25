using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using battleTypes;

public class MilitaryDragAction : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject dragObj;
    [SerializeField]
    private GameObject targetCard;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragObj = gameObject;
        targetCard = null;
        Debug.Log($"{dragObj}を軍事行動ドラッグしました。");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.collider.tag == "BattleCard")
            {
                // 自分以外ならターゲットに設定する
                if(hit.transform.gameObject != gameObject)
                {
                    targetCard = hit.transform.gameObject;
                }
            }
        }

        // ターゲットがいる場合
        if(targetCard != null)
        {
            BattleCard battleCard = targetCard.GetComponent<BattleCard>();
            // ターゲットがカードの場合
            if(battleCard != null)
            {
                Side turnSide = BattleMgr.instance.GetTurnSide();
                // 対象カードがターン側
                if (battleCard.GetSide() == turnSide)
                {
                    // 対象カードがスパイなら
                    if(battleCard.IsHaveAppendKind(BattleCard.AppendKind.eAppendKind_Spy))
                    {
                        // 対象カードを破壊
                        BattleCardCtr.instance.RemoveBattleCard(battleCard);
                    }
                }
                // 対象カードがターンの逆側
                else if(battleCard.GetSide() != turnSide)
                {
                    // 対象カードを破壊
                    BattleCardCtr.instance.RemoveBattleCard(battleCard);
                }

                // BattleMgr更新リクエスト
                BattleMgr.instance.UpdateRequest();
            }
        }

        dragObj = null;
        Debug.Log($"軍事行動ドラッグ終了。");
    }
}
