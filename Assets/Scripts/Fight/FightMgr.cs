using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMgr : MonoBehaviour
{
    // =================構造体================
    public enum eSide
    {
        eSide_Player,
        eSide_Enemy,
        eSide_Max,

        eSide_None = -1,
    }

    // =================変数================
    // インスタンス
    public static FightMgr instance;
    // 側
    private eSide m_Side;

    private void Awake()
    {
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // =================関数================
    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if(instance == null)
        {
            // 作成
            instance = this;
            return true;
        }

        Debug.LogError("FightMgrのインスタンスが生成できませんでした");
        return false;
    }

    // -----側-----
    // 側を設定
    public void SetSide(eSide _side)
    {
        m_Side = _side;
    }
    // 側を取得
    public eSide GetSide()
    {
        return m_Side;
    }
    // 側を取得(indexから)
    public eSide GetSide(int _index)
    {
        // 範囲外ならはじく
        if( _index < 0 || _index > GetSideMax())
        {
            Debug.LogError("範囲外のindex" + "[" + _index.ToString() + "]" + "のため'eSide_None'を返しました" );
            return eSide.eSide_None;
        }

        for(int i = 0; i < (int)eSide.eSide_Max; i++)
        {
            // 指定index以外ははじく
            if(i == _index) { return eSide.eSide_None; }

            // プレイヤー
            if(_index == (int)eSide.eSide_Player)
            {
                return eSide.eSide_Player;
            }
            // 敵
            else if (_index == (int)eSide.eSide_Enemy)
            {
                return eSide.eSide_Enemy;
            }
        }

        return eSide.eSide_None;
    }
    // 指定側のIndexを取得
    public int GetSideIndex(eSide _side)
    {
        return (int)_side;
    }
    // 側の最大数を取得
    public int GetSideMax()
    {
        return (int)eSide.eSide_Max;
    }
}
