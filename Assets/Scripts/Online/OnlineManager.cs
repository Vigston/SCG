using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private const int MAX_PLAYER = 2;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // サーバー接続。
    public void OnlineConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MAX_PLAYER;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public void QuickMatch()
    {
        if(PhotonNetwork.IsConnected == false)
        {
            Debug.LogError("クライアントがサーバーに接続されていないのでクイックマッチを行えませんでした。");
            return;
        }

		if (PhotonNetwork.InLobby == false)
		{
			Debug.LogError("クライアントがロビーに接続されていないのでクイックマッチを行えませんでした。");
            return;
		}

		PhotonNetwork.JoinRandomRoom();
	}

    // サーバーに接続した時。
    public override void OnConnectedToMaster()
    {
        Debug.Log("ConnectMasterServer Success!!!");
    }

    // サーバーとの接続が切断された時。
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"DisconnectMasterServer: {cause.ToString()}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinRoom Success!!!");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MAX_PLAYER)
        {
            Debug.Log("MatchConnecting...");
        }
        else
        {
            Debug.Log("MatchConnect Success!!!");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MAX_PLAYER)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                Debug.Log("MatchConnect MoveBattleScene");
                PhotonNetwork.LoadLevel("BattleScene");
            }
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("CreateRoom Success!!!");
    }
}
