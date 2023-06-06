using UnityEngine;
using UnityEngine.EventSystems;
using battleTypes;

public class GiveJobDrag : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField]
    private BattleCard.JobKind m_giveKind;
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
        // 職業付与を行ったか
        bool isAppendAction = false;

        // 追加種類付与フラグが立っているなら
        if (BattleMgr.instance.IsAddAppendKindFlag())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                // クリックしたオブジェクト
                GameObject hitObj = hit.transform.gameObject;

                // スパイを付与するのは次に相手の場に参加する国民なので例外処理。
                if (m_giveKind == BattleCard.JobKind.eAppendKind_Spy)
                {
                    // 自分のターンなら
                    if(BattleMgr.instance.IsMyTurn())
                    {
                        Side userSide = BattleUserMgr.instance.GetOperateUserSide();

                        BattleArea battleArea = hitObj.GetComponent<BattleArea>();

                        // バトルエリアなら
                        if (battleArea != null)
                        {
                            Side areaSide = battleArea.GetSide();
                            // 相手のバトルエリアなら
                            if (areaSide != userSide)
                            {
                                BattleMgr.instance.SetNextJoinSpyFlag(true);
                                Debug.Log($"次に参加してくる'{battleArea}'の国民は自分のスパイになる");
                                // 付与しました
                                isAppendAction = true;
                            }
                        }
                    }
                }
                else
                {
                    if (hit.collider.tag == "BattleCard")
                    {
                        targetCard = hitObj;
                    }
                }
            }

            // カードにドロップしているなら
            if (targetCard != null)
            {
                // カード種類を設定
                BattleCard battleCard = targetCard.GetComponent<BattleCard>();

                // 自分のターンで自分のカードなら職業付与できる
                if (Common.IsMyTurnAndMyCard(battleCard))
                {
                    // スパイを除く職業が付与されていないなら
                    if (battleCard.GetAppendJobNum(true) <= 0)
                    {
                        // 職業付与
                        battleCard.AppendJob(m_giveKind);
                        // 付与しました
                        isAppendAction = true;
                    }
                }
            }

            // 職業付与を行ったなら
            if(isAppendAction)
            {
                // BattleMgr更新リクエスト
                BattleMgr.instance.UpdateRequest();
                // 追加種類付与カウント
                BattleMgr.instance.AddAppendKindCount();
            }

            dragObj = null;

            // 追加種類付与フラグを初期化
            BattleMgr.instance.SetAddAppendKindFlag(false);

            Debug.Log($"追加種類付与ドラッグ終了。");
        }
    }

}