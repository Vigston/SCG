using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class PhotonViewData : MonoBehaviour, IPunObservable
{
	private PhotonView m_PhotonView;

	private PhotonSyncStatus m_PhotonSyncStatus = PhotonSyncStatus.PHOTON_SYNC_STATUS_WAIT;


	public int firstTurnSide;

	public enum PhotonSyncStatus
	{
		PHOTON_SYNC_STATUS_WAIT,      // 通信同期待ち
		PHOTON_SYNC_STATUS_IN,        // 通信同期中
	}

	public int GetSetFirstTurnSide
	{
		get { return firstTurnSide; }
		set {  firstTurnSide = value; RequestOwner(); }
	}

	void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
	}

	// 通信同期
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		// 通信同期開始
		m_PhotonSyncStatus = PhotonSyncStatus.PHOTON_SYNC_STATUS_IN;

		// オーナーの場合
		if (stream.IsWriting)
		{
			stream.SendNext(firstTurnSide);
		}
		// オーナー以外の場合
		else
		{
			firstTurnSide = (int)stream.ReceiveNext();
		}

		// 通信同期終了
		m_PhotonSyncStatus = PhotonSyncStatus.PHOTON_SYNC_STATUS_WAIT;
	}

	// ネットワークオブジェクトの所有権リクエスト
	private void RequestOwner()
	{
		// 所有権が自分にあるならリクエストの必要がないのではじく。
		if(m_PhotonView.IsMine == true) { return; }

		// リクエストが行えない設定がされているならエラーを流してはじく。
		if(m_PhotonView.OwnershipTransfer != OwnershipOption.Request)
		{
			Debug.LogError("OwnershipTransferをRequestに変更してください。");
			return;
		}

		// 所有権のリクエストを送信
		m_PhotonView.RequestOwnership();
	}
}
