using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCardMgr : MonoBehaviour
{
    // インスタンス
    public static BattleCardMgr instance;

    // カードのPrehab
    public GameObject battleCardPrehab;

    private void Awake()
    {
        CreateInstance();
    }

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

        Debug.LogError("BattleCardMgrのインスタンスが生成できませんでした");
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
