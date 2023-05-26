using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCard : MonoBehaviour
{
    // 基本種類
    public enum Kind
    {
        eKind_People,
        eKind_Max,

        eKind_None = -1,
    }

    // 追加種類
    public enum AppendKind
    {
        eAppendKind_Military,
        eAppendKind_Science,
        eAppendKind_Spy,
        eAppendKind_Max,

        eAppendKind_None = -1,
    }

    ////////////////変数///////////////////////
    [SerializeField]
    private Side m_Side;
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private Kind m_Kind;
    [SerializeField]
    private List<AppendKind> m_AppendKindList;
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
    public AppendKind GetAppendKind(int index)
    {
        return m_AppendKindList[index];
    }
    // 付与種類追加
    public void AddAppendKind(AppendKind _appendKind)
    {
        // 既にこの追加種類を持っているならはじく。
        if (IsHaveAppendKind(_appendKind)) { return; }

        Debug.Log($"{_appendKind}を付与する");

        // 軍事の付与ならMilitaryDragActionをアタッチする
        if(_appendKind == AppendKind.eAppendKind_Military)
        {
            gameObject.AddComponent<MilitaryDragAction>();
        }

        m_AppendKindList.Add(_appendKind);
    }
    // 指定の追加種類を持つか
    public bool IsHaveAppendKind(AppendKind _appendKind)
    {
        return m_AppendKindList.Contains(_appendKind);
    }
    // 付与効果の数を取得
    public int GetAppendKindNum()
    {
        return m_AppendKindList.Count;
    }
    // 全ての付与種類取得
    public List<AppendKind> AllGetAppendKind()
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
        int turnNum = BattleMgr.instance.GetTurnNum();

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
        int turnNum = BattleMgr.instance.GetTurnNum();
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
    public void SetMaterial(AppendKind _apppendKind)
    {

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        switch (_apppendKind)
        {
            case AppendKind.eAppendKind_Military:
                meshRenderer.material = BattleCardMgr.instance.m_MilitaryMaterial;
                break;
            case AppendKind.eAppendKind_Science:
                meshRenderer.material = BattleCardMgr.instance.m_ScienceMaterial;
                break;
            case AppendKind.eAppendKind_Spy:
                meshRenderer.sharedMaterials = new Material[]
                {
                    meshRenderer.sharedMaterial,
                    BattleCardMgr.instance.m_SpyMaterial,
                };
                break;
            default:
                Debug.Log("SetMaterialに登録されていないマテリアルを設定しようとしています");
                break;
        }
    }
}
