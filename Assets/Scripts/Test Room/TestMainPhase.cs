using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestMainPhase : Phase
{
	protected override async UniTask InitState()
	{
		Debug.Log($"{nameof(TestMainPhase)}：{nameof(InitState)}");
		state = eState.Main;
		await UniTask.Yield();
	}

	protected override async UniTask MainState()
	{
		Debug.Log($"{nameof(TestMainPhase)}：{nameof(MainState)}");

		// スペースキーが押されるまで待機
		while (state == eState.Main)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("スペースキーが押されました。EndStateへ遷移。");

				// マスタークライアントがEndStateへの遷移を通知
				if (PhotonNetwork.IsMasterClient)
				{
					photonView.RPC(nameof(ChangeToEndState), RpcTarget.All);
				}
			}

			await UniTask.Yield();
		}

		// フレームの終了を待ってから次のフレームへ
		await UniTask.Yield();
	}

	protected override async UniTask EndState()
	{
		Debug.Log($"{nameof(TestMainPhase)}：EndState");
		await UniTask.Yield();
	}

	[PunRPC]
	private void ChangeToEndState()
	{
		state = eState.End;
	}
}