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
}
