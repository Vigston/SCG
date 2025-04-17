using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;

/// <summary>
/// オンラインマネージャー: Photonを使用したオンライン接続やルーム管理を行うクラス。
/// </summary>
public class Test_OnlineManager : MonoBehaviourPunCallbacks
{
	// 最大プレイヤー数を定義 (2人までのルーム)。
	[SerializeField]
	private const int MAX_PLAYER = 2;

	private void Awake()
	{
		// シーンの同期を自動的に行う設定を有効化。
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Start()
	{
		//if (PlayerPrefs.HasKey("LastRoomName"))
		//{
		//	string lastRoomName = PlayerPrefs.GetString("LastRoomName");
		//	Debug.Log($"保存されたルーム名: {lastRoomName}");

		//	// ここで復帰処理に使う
		//	PhotonNetwork.RejoinRoom(lastRoomName);
		//}
		//else
		//{
		//	Debug.Log("前回のルーム名が見つかりませんでした");
		//}
	}

	/// <summary>
	/// サーバーに接続する。
	/// </summary>
	public void OnlineConnect()
	{
		// UserAuthManager から取得（Steam or Firebase）
		string userId = UserAuthManager.Instance?.UserId;
		bool isFirstLogin = UserAuthManager.Instance?.IsFirstLogin ?? false;

		if (string.IsNullOrEmpty(userId))
		{
			Debug.LogWarning("ユーザーIDが取得できていないため、接続できません。");
			return;
		}

		Debug.Log($"{this}：isFirstLogin = {isFirstLogin}");

		// 認証情報をPhotonに渡す
		PhotonNetwork.AuthValues = new AuthenticationValues(userId);
		PhotonNetwork.NickName = userId;

		// Photon接続開始
		PhotonNetwork.ConnectUsingSettings();
	}

	/// <summary>
	/// 新しいルームを作成する。
	/// </summary>
	public void CreateRoom()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = MAX_PLAYER; // 最大プレイヤー数を設定。
		PhotonNetwork.CreateRoom(null, roomOptions, null);
	}

	/// <summary>
	/// クイックマッチを開始する。
	/// </summary>
	public void QuickMatch()
	{
		if (PhotonNetwork.IsConnected == false)
		{
			Debug.LogError("クライアントがサーバーに接続されていないのでクイックマッチを行えませんでした。");
			return;
		}

		PhotonNetwork.JoinRandomRoom();
	}

	/// <summary>
	/// サーバーに接続成功時に呼び出されるコールバック。
	/// </summary>
	public override void OnConnectedToMaster()
	{
		Debug.Log("ConnectMasterServer Success!!!");
	}

	/// <summary>
	/// サーバーとの接続が切断された時に呼び出されるコールバック。
	/// </summary>
	/// <param name="cause">切断の原因。</param>
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log($"DisconnectMasterServer: {cause.ToString()}");
	}

	/// <summary>
	/// ランダムルームへの参加に失敗した場合のコールバック。
	/// 新しいルームを作成する。
	/// </summary>
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		CreateRoom();
	}

	/// <summary>
	/// ルームへの参加成功時に呼び出されるコールバック。
	/// </summary>
	public override void OnJoinedRoom()
	{
		Debug.Log($"{nameof(OnJoinedRoom)}：JoinRoom Success!!!");

		string currentRoomName = PhotonNetwork.CurrentRoom.Name;

		// ルーム名を保存
		PlayerPrefs.SetString("LastRoomName", currentRoomName);
		PlayerPrefs.Save(); // 保存を即反映

		Debug.Log($"{nameof(OnJoinedRoom)}：ルーム名を保存: {currentRoomName}");
	}

	/// <summary>
	/// 他のプレイヤーがルームに参加した時に呼び出されるコールバック。
	/// </summary>
	/// <param name="newPlayer">参加したプレイヤー。</param>
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			// 現在のプレイヤー数が最大プレイヤー数に達した場合
			if (PhotonNetwork.CurrentRoom.PlayerCount == MAX_PLAYER)
			{
				PhotonNetwork.CurrentRoom.IsOpen = false; // ルームを閉じる。
				Debug.Log("MatchConnect MoveTestRoom_Battle");
				PhotonNetwork.LoadLevel("TestRoom_Battle"); // バトルシーンに移動。
			}
			else
			{
				Debug.Log("Waiting for more players to join...");
			}
		}
	}

	/// <summary>
	/// ルーム作成成功時に呼び出されるコールバック。
	/// </summary>
	public override void OnCreatedRoom()
	{
		Debug.Log("CreateRoom Success!!!");
	}
}