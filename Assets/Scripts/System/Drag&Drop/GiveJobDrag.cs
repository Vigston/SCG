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
        // 追加付与が行えるなら
        if(BattleMgr.instance.IsPlayAddAppendKind())
        {
            // 追加種類付与フラグを立てる
            BattleMgr.instance.SetAddAppendKindFlag(true);
        }

        // 追加種類付与フラグが立っているなら
        if (BattleMgr.instance.IsAddAppendKindFlag())
        {
            dragObj = gameObject;
            targetCard = null;
            Debug.Log($"{dragObj}を追加種類付与ドラッグしました。");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 追加種類付与フラグが立っているなら
        if (BattleMgr.instance.IsAddAppendKindFlag())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                // スパイを付与するのは次に相手の場に参加する国民なので例外処理。
                if (m_giveKind == BattleCard.AppendKind.eAppendKind_Spy)
                {
                    // ターンとカードの側が一緒なら職業付与する
                    Side turnSide = BattleMgr.instance.GetTurnSide();
                    if (turnSide == Side.eSide_Player)
                    {
                        if (hit.collider.tag == "EnemyArea")
                        {
                            BattleMgr.instance.SetNextJoinSpyFlag(true);
                            Debug.Log("次に参加してくる相手の国民は自分のスパイになる");
                        }
                    }
                    else if (turnSide == Side.eSide_Enemy)
                    {
                        if (hit.collider.tag == "PlayerArea")
                        {
                            BattleMgr.instance.SetNextJoinSpyFlag(true);
                            Debug.Log("次に参加してくる自分の国民は相手のスパイになる");
                        }
                    }
                }
                else
                {
                    if (hit.collider.tag == "BattleCard")
                    {
                        targetCard = hit.transform.gameObject;
                    }
                }
            }

            // カードにドロップしているなら
            if (targetCard != null)
            {
                // カード種類を設定
                BattleCard battleCard = targetCard.GetComponent<BattleCard>();

                // ターンとカードの側が一緒なら職業付与する
                Side turnSide = BattleMgr.instance.GetTurnSide();
                if (turnSide == battleCard.GetSide())
                {
                    // カードの種類設定
                    battleCard.AddAppendKind(m_giveKind);

                    // BattleMgr更新リクエスト
                    BattleMgr.instance.UpdateRequest();
                    // 追加種類付与カウント
                    BattleMgr.instance.AddAppendKindCount();
                }

            }

            dragObj = null;

            // 追加種類付与フラグを初期化
            BattleMgr.instance.SetAddAppendKindFlag(false);

            Debug.Log($"追加種類付与ドラッグ終了。");
        }
    }

}