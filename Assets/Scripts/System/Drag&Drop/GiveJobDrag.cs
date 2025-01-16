using UnityEngine;
using UnityEngine.EventSystems;
using battleTypes;

public class GiveJobDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private BattleCard.JobKind m_GiveKind;
    [SerializeField]
    private GameObject m_DragObj;
    [SerializeField]
    private GameObject m_TargetCard;
    [SerializeField]
    private bool m_GiveJobDragFlag = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 現在のフェイズオブジェクト
        GameObject phaseObj = BattleMgr.instance.GetPhaseObject();
        // メインフェイズ
		MainPhase mainPhase = phaseObj.GetComponent<MainPhase>();

		// メインフェイズじゃないならはじく
		if (mainPhase == null)
        {
			return;
        }

        // メインステートじゃないならはじく
        if(mainPhase.GetSetState != MainPhase.State.eState_Main)
        {
            return;
        }

        /////職業付与処理/////
        GetSetGiveJobDragFlag = true;
		m_DragObj = gameObject;
		m_TargetCard = null;
		Debug.Log($"{m_DragObj}を追加種類付与ドラッグしました。");
	}

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 職業付与を行ったか
        bool isAppendAction = false;

        // 追加種類付与フラグが立っているなら
        if (GetSetGiveJobDragFlag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                // クリックしたオブジェクト
                GameObject hitObj = hit.transform.gameObject;

                // スパイを付与するのは次に相手の場に参加する国民なので例外処理。
                if (m_GiveKind == BattleCard.JobKind.eAppendKind_Spy)
                {
                    // 自分のターンなら
                    if (Common.IsMyTurn())
                    {
                        Side userSide = BattleUserMgr.instance.GetSetOperateSide;

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
                        m_TargetCard = hitObj;
                    }
                }
            }

            // カードにドロップしているなら
            if (m_TargetCard != null)
            {
                // カード種類を設定
                BattleCard battleCard = m_TargetCard.GetComponent<BattleCard>();

                // 自分のターンで自分のカードなら職業付与できる
                if (Common.IsMyTurnAndMyCard(battleCard))
                {
                    // スパイを除く職業が付与されていないなら
                    if (battleCard.GetAppendJobNum(true) <= 0)
                    {
                        // 職業付与
                        battleCard.AppendJob(m_GiveKind);
                        // 付与しました
                        isAppendAction = true;
                    }
                }
            }

            // 職業付与を行ったなら
            if (isAppendAction)
            {
                // BattleMgr更新リクエスト
                BattleMgr.instance.UpdateRequest();
            }

            m_DragObj = null;

			// 追加種類付与フラグを初期化
			GetSetGiveJobDragFlag = false;
        }
    }

    public bool GetSetGiveJobDragFlag
    {
        get { return m_GiveJobDragFlag; }
        set { m_GiveJobDragFlag = value; }
    }
}