using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using battleTypes;

public class ArmamentDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
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

                // 自分のターンで自分のカードなら職業付与できる
                if (Common.IsMyTurnAndMyCard(battleCard))
                {
                    Military military = battleCard.GetMilitary();

                    // 軍事カードじゃないならはじく
                    if(military == null) { return; }

                    // 武装付与
                    military.SetStatus(Military.Status.eStatus_Armed);
                    battleCard.SetMaterial(BattleCard.JobKind.eAppendKind_Military);
                }
            }

            Debug.Log("武装付与ドラッグ終了。");
        }
    }

    // 武装付与が行えるか
    public bool IsArmament()
    {
        // 行える
        return true;
    }
}
