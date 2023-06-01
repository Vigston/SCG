using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleUserCtr : MonoBehaviour
{
    // =================変数=================
    // インスタンス
    public static BattleUserCtr instance;

    private void Awake()
    {
        // インスタンス生成
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

        Debug.LogError("BattleUserCtrのインスタンスが生成できませんでした");
        return false;
    }

    // プレイヤーユーザー作成
    public void CreatePlayerUser()
    {
        // ゲームオブジェクト生成
        GameObject playerUserObj = new GameObject();

        // ユーザークラス追加
        playerUserObj.AddComponent<BattleUser>();

        // オブジェクト名設定
        playerUserObj.name = "User_Player";

        BattleUser battleUser = playerUserObj.GetComponent<BattleUser>();

        // バトルユーザーがないならはじく
        if(battleUser == null) { return; }

        // バトルユーザー設定
        battleUser.SetSide(Side.eSide_Player);

        // BattleUserMgrに設定
        BattleUserMgr.instance.SetPlayerUser(battleUser);
    }

    // 敵ユーザー作成
    public void CreateEnemyUser()
    {
        // ゲームオブジェクト生成
        GameObject enemyUserObj = new GameObject();

        // ユーザークラス追加
        enemyUserObj.AddComponent<BattleUser>();

        // オブジェクト名設定
        enemyUserObj.name = "User_Enemy";

        BattleUser battleUser = enemyUserObj.GetComponent<BattleUser>();

        // バトルユーザーがないならはじく
        if (battleUser == null) { return; }

        // バトルユーザー設定
        battleUser.SetSide(Side.eSide_Enemy);

        // BattleUserMgrに設定
        BattleUserMgr.instance.SetEnemyUser(battleUser);
    }
}
