using Photon.Pun;
using UnityEngine;

using battleTypes;
using Photon.Realtime;
using static BattleMgr;
using UnityEngine.Rendering;
using System.IO;

// ネットワーク通信同期オブジェクト
public class NetWorkSync : MonoBehaviourPun
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
		// 通信同期の実行条件を満たしていないなら行わない
		if(IsExeNetworkSync() == false) { return false; }

		/////通信同期/////
		// ターン数
		photonView.RPC("SetSyncTurnNum", RpcTarget.Others, BattleMgr.instance.GetSetTurnNum);
		// ターン側
		photonView.RPC("SetSyncTurnSide", RpcTarget.Others, BattleMgr.instance.GetSetTurnSide);
		// 勝敗
		photonView.RPC("SetSyncBattleResult", RpcTarget.Others, BattleMgr.instance.GetSetBattleResult);
		// 操作側
		photonView.RPC("SetSyncOperateUserSide", RpcTarget.Others, BattleUserMgr.instance.GetSetOperateSide);

		Debug.Log($"通信同期が終了しました：ActorNumber[{PhotonNetwork.LocalPlayer.ActorNumber}]");
		// 正常に終了している
		return true;
	}

	// 通信同期の実行条件を満たしているか
	public bool IsExeNetworkSync()
	{
		// 操作ユーザー
		BattleUser operateUser = BattleUserMgr.instance.GetSetOperateUser;

		// 通信接続ができていないならはじく。
		if(PhotonNetwork.IsConnected == false)
		{
			Debug.Log($"通信接続が正常に行われていないので通信同期を行いません。通信接続状況：{PhotonNetwork.IsConnected}");
			return false;
		}

		// 操作側のクライアントじゃないならはじく。
		if (operateUser.GetSetNetWorkActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			Debug.Log($"操作側のクライアントじゃないので通信同期を行いません。操作側ActorNumber：{operateUser.GetSetNetWorkActorNumber}｜ローカルActorNumber：{PhotonNetwork.LocalPlayer.ActorNumber}");
			return false;
		}

		// 切断されていたら通信同期失敗としてはじく。
		if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
		{
			Debug.LogError($"通信が切断されているので通信同期を行えませんでした。[NetworkClientState：{PhotonNetwork.NetworkClientState}]");
			return false;
		}

		// 通信同期可能
		return true;
	}

	// 相手側にターン数の同期を行う
	[PunRPC]
	public void SetSyncTurnNum(int turnNum)
	{
		Debug.Log($"通信同期(SetSyncTurnNum)：{turnNum}");
		if(BattleMgr.instance.GetSetTurnNum != turnNum)
		{
			Debug.Log($"通信同期取れていません(SetSyncTurnNum)：[自分]{BattleMgr.instance.GetSetTurnNum},[相手]{turnNum}");
		}
	}
	// 相手側にターン側の同期を行う
	[PunRPC]
	public void SetSyncTurnSide(int turnSide)
	{
		Debug.Log($"通信同期(SetSyncTurnSide)：{turnSide}");
		if (BattleMgr.instance.GetSetTurnSide != Common.GetRevSide((Side)turnSide))
		{
			Debug.Log($"通信同期取れていません(GetSetTurnSide)：[自分]{BattleMgr.instance.GetSetTurnSide},[相手]{Common.GetRevSide((Side)turnSide)}");
		}
	}
	// 相手側にフェイズの同期を行う
	[PunRPC]
	public void SetSyncPhaseType(int phaseType)
	{
		Debug.Log($"通信同期(SetSyncPhaseType)：{phaseType}");
		if (BattleMgr.instance.GetSetPhaseType != (PhaseType)phaseType)
		{
			Debug.Log($"通信同期取れていません(GetSetPhaseType)：[自分]{BattleMgr.instance.GetSetPhaseType},[相手]{(PhaseType)phaseType}");
		}
	}
	// 相手側に勝敗の同期を行う
	[PunRPC]
	public void SetSyncBattleResult(int battleResult)
	{
		Debug.Log($"通信同期(SetSyncBattleResult)：{battleResult}");
		if (BattleMgr.instance.GetSetBattleResult != (BattleResult)battleResult)
		{
			Debug.Log($"通信同期取れていません(GetSetPhaseType)：[自分]{BattleMgr.instance.GetSetBattleResult},[相手]{(BattleResult)battleResult}");
		}
	}
	
	// 相手側に操作側プレイヤーの同期を行う
	[PunRPC]
	public void SetSyncOperateUserSide(int operateUserSide)
	{
		Debug.Log($"通信同期(SetSyncOperateUserSide)：{operateUserSide}");
		if (BattleUserMgr.instance.GetSetOperateSide != Common.GetRevSide((Side)operateUserSide))
		{
			Debug.Log($"通信同期取れていません(GetSetPhaseType)：[自分]{BattleUserMgr.instance.GetSetOperateSide},[相手]{Common.GetRevSide((Side)operateUserSide)}");
		}
	}
}
