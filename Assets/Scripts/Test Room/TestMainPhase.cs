using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestMainPhase : Phase
{
	protected override async UniTask StartState()
	{
		// 基底処理実行
		await base.StartState();
		await UniTask.CompletedTask;
	}

	protected override async UniTask MainState()
	{
		// 基底処理実行
		await base.MainState();

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
		{
			Debug.Log("スペースキーが押されました。EndStateへ遷移。");

			// マスタークライアントがEndStateへの遷移を通知
			if (PhotonNetwork.IsMasterClient)
			{
				photonView.RPC(nameof(ChangeToEndState), RpcTarget.All);
			}
		}

		await UniTask.CompletedTask;
	}

	protected override async UniTask EndState()
	{
		// 基底処理実行
		await base.EndState();
		await UniTask.CompletedTask;
	}

	[PunRPC]
	private void ChangeToEndState()
	{
		GetSetState = eState.End;
	}
}