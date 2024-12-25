using Photon.Pun;
using UnityEngine;

using battleTypes;
using Photon.Realtime;
using static BattleMgr;

// ネットワーク通信同期オブジェクト
public class NetWorkSync : MonoBehaviourPunCallbacks
{
	public enum NetworkSyncStatus
	{
		PHOTON_SYNC_STATUS_WAIT,      // 通信同期待ち
		PHOTON_SYNC_STATUS_IN,        // 通信同期中
	}

	public NetworkSyncStatus m_NetworkSyncStatus = NetworkSyncStatus.PHOTON_SYNC_STATUS_WAIT;

	// インスタンス
	public static NetWorkSync instance;

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

		Debug.LogError("NetWorkSyncのインスタンスが生成できませんでした");
		return false;
	}

	// ゲーム情報の通信同期処理(※重いので多用厳禁※)
	public bool GameInfoNetworkSync()
	{
		// マスタークライアント以外は処理を行わない。
		if (PhotonNetwork.IsMasterClient == false) { return false; }

		// 切断されていたら通信同期失敗としてはじく。
		if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
		{
			Debug.LogError($"通信が切断されているので通信同期を行えませんでした。[NetworkClientState：{PhotonNetwork.NetworkClientState}]");
			return false;
		}

		/////通信同期/////
		// ターン数
		photonView.RPC("SetSyncTurnNum", RpcTarget.Others, BattleMgr.instance.GetSetTurnNum);
		// ターン側
		photonView.RPC("SetSyncTurnSide", RpcTarget.Others, BattleMgr.instance.GetSetTurnSide);
		// フェイズ
		photonView.RPC("SetSyncPhaseType", RpcTarget.Others, BattleMgr.instance.GetSetPhaseType);
		// 勝敗
		photonView.RPC("SetSyncBattleResult", RpcTarget.Others, BattleMgr.instance.GetSetBattleResult);
		// 操作側
		//photonView.RPC("SetSyncOperateUserSide", RpcTarget.Others, BattleUserMgr.instance.GetSetOperateUserSide);

		Debug.Log($"通信同期が終了しました：ActorNumber[{PhotonNetwork.LocalPlayer.ActorNumber}]");
		// 正常に終了している
		return true;
	}

	// 相手側にターン数の同期を行う
	[PunRPC]
	public void SetSyncTurnNum(int turnNum)
	{
		BattleMgr.instance.GetSetTurnNum = turnNum;
	}
	// 相手側にターン側の同期を行う
	[PunRPC]
	public void SetSyncTurnSide(Side turnSide)
	{
		BattleMgr.instance.GetSetTurnSide = Common.GetRevSide(turnSide);
	}
	// 相手側にフェイズの同期を行う
	[PunRPC]
	public void SetSyncPhaseType(PhaseType phaseType)
	{
		BattleMgr.instance.GetSetPhaseType = phaseType;
	}
	// 相手側に勝敗の同期を行う
	[PunRPC]
	public void SetSyncBattleResult(BattleResult battleResult)
	{
		BattleMgr.instance.GetSetBattleResult = battleResult;
	}
	/*
	// 相手側に操作側プレイヤーの同期を行う
	[PunRPC]
	public void SetSyncOperateUserSide(Side operateUserSide)
	{
		BattleUserMgr.instance.GetSetOperateUserSide = Common.GetRevSide(operateUserSide);
	}
	*/
}
