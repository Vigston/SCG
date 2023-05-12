using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using TMPro;

public class BattleMgr : MonoBehaviour
{
    // =================構造体================
    enum PhaseType
    {

    }

    // =================変数================
    // インスタンス
    public static BattleMgr instance;
    // ターン側
    [SerializeField]
    private Side m_TurnSide;

    public TextMeshProUGUI text;

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
        text.text = "ターン側 : " + m_TurnSide.ToString();
    }

    // =================関数================
    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if (instance == null)
        {
            // 作成
            instance = this;
        }

        // インスタンスが作成済みなら終了
        if (instance != null) { return true; }

        Debug.LogError("BattleMgrのインスタンスが生成できませんでした");
        return false;
    }
    // --システム--
    // ターン終了
    public void TurnEnd()
    {
        Side revSide = Common.GetRevSide(m_TurnSide);

        m_TurnSide = revSide;
    }

    // -----側-----
    // ターン側を設定
    public void SetSide(Side _side)
    {
        m_TurnSide = _side;
    }
    // ターン側を取得
    public Side GetSide()
    {
        return m_TurnSide;
    }
    // 側を取得(indexから)
    public Side GetSide(int _index)
    {
        // 範囲外ならはじく
        if( _index < 0 || _index > GetSideMax())
        {
            Debug.LogError("範囲外のindex" + "[" + _index.ToString() + "]" + "のため'eSide_None'を返しました" );
            return Side.eSide_None;
        }

        for(int i = 0; i < (int)Side.eSide_Max; i++)
        {
            // 指定index以外ははじく
            if(i == _index) { return Side.eSide_None; }

            // プレイヤー
            if(_index == (int)Side.eSide_Player)
            {
                return Side.eSide_Player;
            }
            // 敵
            else if (_index == (int)Side.eSide_Enemy)
            {
                return Side.eSide_Enemy;
            }
        }

        return Side.eSide_None;
    }
    // 指定側のIndexを取得
    public int GetSideIndex(Side _side)
    {
        return (int)_side;
    }
    // 側の最大数を取得
    public int GetSideMax()
    {
        return (int)Side.eSide_Max;
    }
}
