using UnityEngine;
using UnityEngine.EventSystems;
using battleTypes;

public class GiveJobDrag : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField]
    private BattleCard.AppendKind m_giveKind;
    [SerializeField]
    private GameObject dragObj;

    [SerializeField]
    private GameObject targetCard;
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragObj = gameObject;
        targetCard = null;
        Debug.Log($"{dragObj}をドラッグしました。");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            // スパイを付与するのは次に相手の場に参加する国民なので例外処理。
            if(m_giveKind == BattleCard.AppendKind.eAppendKind_Spy)
            {
                if(hit.collider.tag == "EnemyArea")
                {

                }
            }
            else
            {
                if (hit.collider.tag == "BattleCard")
                {
                    targetCard = hit.transform.gameObject;
                    Debug.Log(hit.transform.gameObject.ToString());
                }
            }
        }

        // カードにドロップしているなら
        if(targetCard != null)
        {
            // カード種類を設定
            BattleCard battleCard = targetCard.GetComponent<BattleCard>();

            // ターンとカードの側が一緒なら職業付与する
            Side turnSide = BattleMgr.instance.GetTurnSide();
            if(turnSide == battleCard.GetSide())
            {
                // カードの種類設定
                battleCard.AddAppendKind(m_giveKind);

                // BattleMgr更新リクエスト
                BattleMgr.instance.UpdateRequest();
            }
            
        }

        dragObj = null;

        Debug.Log($"ドラッグ終了。");
    }

}