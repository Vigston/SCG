using UnityEngine;
using Photon.Pun;

public class Test_NetWorkMgr : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    //////////////////////////
    // ===== ユーザー ===== //
    //////////////////////////
    // ユーザー情報の通信同期
    [PunRPC]
    private void RPC_SyncUser_MC()
    {
        // マスタークライアントならはじく
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError($"{nameof(RPC_SyncUser_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
            return;
        }
    }

    // ユーザー情報の送信
    [PunRPC]
    private void RPC_PushUser_CM()
    {
        // マスタークライアントじゃないならはじく
        if(!PhotonNetwork.IsMasterClient)
        {
			Debug.LogError($"{nameof(RPC_PushUser_CM)}非マスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}
    }
}