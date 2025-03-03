using UnityEngine;
using battleTypes;
using Photon.Pun;

public class Test_UserCtr : MonoBehaviour
{
    // =================変数=================
    // インスタンス
    public static Test_UserCtr instance;

    private void Awake()
    {
        // インスタンス生成
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

        Debug.LogError($"{this}のインスタンスが生成できませんでした");
        return false;
    }

    // プレイヤーユーザー作成
    public void CreatePlayerUser()
    {
        // ゲームオブジェクト生成
        GameObject playerUserObj = new GameObject();

        // ユーザークラス追加
        playerUserObj.AddComponent<Test_User>();

        // オブジェクト名設定
        playerUserObj.name = "User_Player";

		Test_User user = playerUserObj.GetComponent<Test_User>();

        // バトルユーザーがないならはじく
        if(user == null) { return; }

		// == ユーザー設定 ==
		user.GetSetSide = Side.eSide_Player;                                // 側
		user.GetSetID = PhotonNetwork.LocalPlayer.ActorNumber;   // プレイヤーID

		// UserMgrに設定
		Test_UserMgr.instance.GetSetPlayerUser = user;
    }

    // 敵ユーザー作成
    public void CreateEnemyUser()
    {
        // ゲームオブジェクト生成
        GameObject enemyUserObj = new GameObject();

        // ユーザークラス追加
        enemyUserObj.AddComponent<Test_User>();

        // オブジェクト名設定
        enemyUserObj.name = "User_Enemy";

		Test_User battleUser = enemyUserObj.GetComponent<Test_User>();

        // ユーザーがないならはじく
        if (battleUser == null) { return; }

        // バトルユーザー設定
        battleUser.GetSetSide = Side.eSide_Enemy;
		// プレイヤーのActorNumberを基に敵のActorNumberを設定
		int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		int enemyActorNumber = (playerActorNumber == 1) ? 2 : 1;
		battleUser.GetSetID = enemyActorNumber;

		// UserMgrに設定
		Test_UserMgr.instance.GetSetEnemyUser = battleUser;
    }
}
