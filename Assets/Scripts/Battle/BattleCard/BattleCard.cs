using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCard : MonoBehaviour
{
    // ステータス
    public enum Status
    {
        // 通常
        eStatus_Normal,
        // 疲労
        eStatus_Fatigue,
    }

    // 基本種類
    public enum Kind
    {
        eKind_People,
        eKind_Max,

        eKind_None = -1,
    }

    // 追加種類
    public enum JobKind
    {
        eAppendKind_Military,
        eAppendKind_Science,
        eAppendKind_Spy,
        eAppendKind_Merchant,
        eAppendKind_Max,

        eAppendKind_None = -1,
    }

    ////////////////変数///////////////////////
    [SerializeField]
    private Side m_Side;
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private Status m_Status;
    [SerializeField]
    private Kind m_Kind;
    [SerializeField]
    private List<JobKind> m_AppendKindList;
    [SerializeField]
    private bool m_IsEnable = true;
    [SerializeField]
    private bool m_IsDraw = true;
    [SerializeField]
    private int m_EntryTurn = 0;
    [SerializeField]
    private const int m_LimitedActionNum = 1;
    [SerializeField]
    private int m_ActionNum = 0;

    [SerializeField]
    Military m_Military;
    [SerializeField]
    Science m_Science;
    [SerializeField]
    Spy m_Spy;
    [SerializeField]
    Merchant m_Merchant;

    private void Awake()
    {
        m_Military = null;
        m_Science = null;
        m_Spy = null;
        m_Merchant = null;
    }

    void Start()
    {
        // 描画処理
        CheckDrawFlag(this.GetCancellationTokenOnDestroy()).Forget();
    }

    // ---システム---
    // 描画フラグ処理
    async UniTask CheckDrawFlag(CancellationToken token)
    {
        // ゲームオブジェクトが破棄されているならはじく
        await UniTask.Yield(token);
        // 描画フラグが立っていたらはじく
        await UniTask.WaitUntil(() => !IsDraw());

        // 描画無効化処理
        Renderer renderer = GetComponent<Renderer>();
        if (renderer.enabled)
        {
            renderer.enabled = false;
        }
    }

    // ターン情報初期化
    public void InitTurnInfo()
    {
        // ターン中行動回数初期化
        InitActionNum();
    }

    ////////////////関数///////////////////////
    // ---側---
    // 側設定
    public void SetSide(Side _side)
    {
        m_Side = _side;
    }
    // 側取得
    public Side GetSide()
    {
        return m_Side;
    }
    // ---位置---
    // 位置設定
    public void SetPosiiton(Position _posiiton)
    {
        m_Position = _posiiton;
    }
    // 位置取得
    public Position GetPosition()
    {
        return m_Position;
    }
    // ---ステータス---
    // ステータス設定
    public void SetStatus(Status _status)
    {
        m_Status = _status;
    }
    // ステータス取得
    public Status GetStatus()
    {
        return m_Status;
    }
    // 指定のステータスか
    public bool IsStatus(Status _status)
    {
        return m_Status == _status;
    }

    // ---種類---
    // 種類設定
    public void SetKind(Kind _kind)
    {
        m_Kind = _kind;
    }
    // 種類取得
    public Kind GetKind()
    {
        return m_Kind;
    }
    // 付与種類取得
    public JobKind GetAppendKind(int index)
    {
        return m_AppendKindList[index];
    }
    // 付与種類追加
    public void AddAppendKind(JobKind _jobKind)
    {
        // 既にこの追加種類を持っているならはじく。
        if (IsHaveAppendKind(_jobKind)) { return; }

        m_AppendKindList.Add(_jobKind);
    }
    // 付与種類削除
    public void RemoveAppendKind(JobKind _jobKind)
    {
        // この付与種類を持っていないならはじく
        if (!IsHaveAppendKind(_jobKind)) { return; }

        m_AppendKindList.Remove(_jobKind);
    }
    // 指定の追加種類を持つか
    public bool IsHaveAppendKind(JobKind _appendKind)
    {
        return m_AppendKindList.Contains(_appendKind);
    }
    // 付与効果の数を取得
    public int GetAppendKindNum()
    {
        return m_AppendKindList.Count;
    }
    // 全ての付与種類取得
    public List<JobKind> AllGetAppendKind()
    {
        return m_AppendKindList;
    }
    // 全ての付与種類削除
    public void AllRemoveAppendKind()
    {
        m_AppendKindList.Clear();
    }
    // ---有効フラグ---
    // 有効フラグ設定
    public void SetEnable(bool _enable)
    {
        m_IsEnable = _enable;
    }
    // 有効フラグが立っているか
    public bool IsEnable()
    {
        return m_IsEnable;
    }
    // 描画フラグを設定
    public void SetIsDraw(bool isDraw)
    {
        m_IsDraw = isDraw;
    }
    // 描画するかどうか
    public bool IsDraw()
    {
        return m_IsDraw;
    }
    // 登場したターン設定
    public void SetEntryTurn()
    {
        int turnNum = BattleMgr.instance.GetSetTurnNum;

        m_EntryTurn = turnNum;
    }
    // 登場したターン取得
    public int GetEntryTurn()
    {
        return m_EntryTurn;
    }
    // このターンに登場したか
    public bool IsEntryThisTurn()
    {
        int turnNum = BattleMgr.instance.GetSetTurnNum;
        int entryTurn = GetEntryTurn();

        return turnNum == entryTurn;
    }
    // 行動した回数を初期化
    public void InitActionNum()
    {
        m_ActionNum = 0;
    }
    // 行動した回数を追加
    public void AddActionNum()
    {
        m_ActionNum += 1;
    }
    // 行動した回数を取得
    public int GetActionNum()
    {
        return m_ActionNum;
    }
    // 行動できるか
    public bool IsAction()
    {
        int actionNum = GetActionNum();

        // 行動可能数を超えるなら行動できない
        if(actionNum >= m_LimitedActionNum)
        {
            // 行動できない
            return false;
        }

        // 行動できる
        return true;
    }

    // マテリアル設定
    public void SetMaterial(JobKind _apppendKind)
    {

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        switch (_apppendKind)
        {
            case JobKind.eAppendKind_Military:
                Debug.Log("軍事カードマテリアル設定！！！");
                Military military = gameObject.GetComponent<Military>();
                if(military != null)
                {
                    Military.Status status = military.GetStatus();
                    Debug.Log($"{status}のマテリアル設定！！");
                    switch (status)
                    {
                        case Military.Status.eStatus_Unarmed:
                            meshRenderer.material = BattleCardMgr.instance.m_MilitaryUnarmedMaterial;
                            break;
                        case Military.Status.eStatus_Armed:
                            meshRenderer.material = BattleCardMgr.instance.m_MilitaryArmedMaterial;
                            break;
                        default:
                            Debug.Log("未登録のマテリアルを検知！！");
                            break;
                    }
                }
                else
                {
                    Debug.Log("軍事カードが取得できない！！！");
                }
                break;
            case JobKind.eAppendKind_Science:
                meshRenderer.material = BattleCardMgr.instance.m_ScienceMaterial;
                break;
            case JobKind.eAppendKind_Spy:
                meshRenderer.sharedMaterials = new Material[]
                {
                    meshRenderer.sharedMaterial,
                    BattleCardMgr.instance.m_SpyMaterial,
                };
                break;
            case JobKind.eAppendKind_Merchant:
                meshRenderer.material = BattleCardMgr.instance.m_MerchantMaterial;
                break;
            default:
                Debug.Log("SetMaterialに登録されていないマテリアルを設定しようとしています");
                break;
        }
    }

    // 軍事を取得する
    public Military GetMilitary()
    {
        return m_Military;
    }
    // 研究を取得する
    public Science GetScience()
    {
        return m_Science;
    }
    // スパイを取得する
    public Spy GetSpy()
    {
        return m_Spy;
    }
    // 商人を取得する
    public Merchant GetMerchant()
    {
        return m_Merchant;
    }

    // 職業を付与する
    public void AppendJob(JobKind _jobKind)
    {
        // スパイを除く職業が付与されているならはじく
        if (GetAppendJobNum(true) >= 1) { return; }

        // 既にこの職業が付与されているならはじく
        if (IsAppendJob(_jobKind)) { return; }

        // 付与
        switch (_jobKind)
        {
            case JobKind.eAppendKind_Military:
                // 非武装状態でアタッチ
                m_Military = gameObject.AddComponent<Military>();
                m_Military.SetStatus(Military.Status.eStatus_Unarmed);
                // 軍事の付与なのでMilitaryDragActionをアタッチする
                gameObject.AddComponent<MilitaryDragAction>();
                Debug.Log("軍事の付与");
                break;
            case JobKind.eAppendKind_Science:
                m_Science = gameObject.AddComponent<Science>();
                Debug.Log("研究の付与");
                break;
            case JobKind.eAppendKind_Spy:
                m_Spy = gameObject.AddComponent<Spy>();
                Debug.Log("スパイの付与");
                break;
            case JobKind.eAppendKind_Merchant:
                m_Merchant = gameObject.AddComponent<Merchant>();
                Debug.Log("商人の付与");
                break;
            default:
                Debug.Log($"登録されていない職業({_jobKind})を付与しようとしているので確認お願いします");
                break;
        }

        // マテリアル設定
        SetMaterial(_jobKind);

        // 付与職業種類にも設定
        AddAppendKind(_jobKind);
    }

    // 職業を全て削除する
    public void AllRemoveAppendJob()
    {
        for (int i = 0; i < (int)JobKind.eAppendKind_Max; i++)
        {
            // 職業削除
            RemoveAppendJob((JobKind)i);
        }

        // 全ての付与職業種類削除
        AllRemoveAppendKind();
    }

    // 職業を削除する
    public void RemoveAppendJob(JobKind _jobKind)
    {
        // 指定の職業が付与されていないならはじく
        if (!IsAppendJob(_jobKind)) { return; }

        // 削除
        switch (_jobKind)
        {
            case JobKind.eAppendKind_Military:
                m_Military = null;
                break;
            case JobKind.eAppendKind_Science:
                m_Science = null;
                break;
            case JobKind.eAppendKind_Spy:
                m_Spy = null;
                break;
            case JobKind.eAppendKind_Merchant:
                m_Merchant = null;
                break;
            default:
                break;
        }

        // 付与職業種類削除
        RemoveAppendKind(_jobKind);
    }

    // 職業の付与されている数を取得
    public int GetAppendJobNum(bool isNoSpy)
    {
        int appendJobNum = 0;

        for(int i = 0; i < (int)JobKind.eAppendKind_Max; i++)
        {
            // スパイを除くフラグが立っている場合
            if(isNoSpy)
            {
                // スパイならはじく
                if((JobKind)i == JobKind.eAppendKind_Spy) { continue; }
            }

            // 付与されていないならはじく
            if (!IsAppendJob((JobKind)i)) { continue; }

            // 付与されている数を加算
            appendJobNum++;
        }

        return appendJobNum;
    }

    // 指定の職業が付与されているか
    public bool IsAppendJob(JobKind _jobKind)
    {
        switch(_jobKind)
        {
            case JobKind.eAppendKind_Military:
                if(m_Military != null) { return true; }
                break;
            case JobKind.eAppendKind_Science:
                if (m_Science != null) { return true; }
                break;
            case JobKind.eAppendKind_Spy:
                if (m_Spy != null) { return true; }
                break;
            case JobKind.eAppendKind_Merchant:
                if (m_Merchant != null) { return true; }
                break;
            default:
                Debug.Log("指定の職業について記載がありませんので確認してください");
                break;
        }

        // その職業は付与されていない
        return false;
    }
}
