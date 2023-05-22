using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleCard : MonoBehaviour
{
    // 種類
    public enum Kind
    {
        eKind_People,
        eKind_Military,
        eKind_Science,
        eKind_Spy,
        eKind_Agent,
        eKind_Max,

        eKind_None = -1,
    }

    ////////////////変数///////////////////////
    [SerializeField]
    private Side m_Side;
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private Kind m_Kind;
    [SerializeField]
    private bool m_IsEnable = true;
    [SerializeField]
    private bool m_IsDraw = true;

    void Start()
    {
        // 描画処理
        CheckDrawFlag(this.GetCancellationTokenOnDestroy()).Forget();
    }

    // ---システム---
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
}
