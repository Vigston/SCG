using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;

public class StepManager : MonoBehaviourPunCallbacks
{
	public Step currentStep;
	public bool isMaster => PhotonNetwork.IsMasterClient;
	public bool isStepReady = false;
	public bool isPlayerTurn;

	// 現在のターン数
	public int currentTurn = 1;

	private void Start()
	{
		if (isMaster)
		{
			// ゲーム開始時に全プレイヤーのステップを `StartStep` に統一
			photonView.RPC(nameof(SetStep), RpcTarget.All, 0);
			isPlayerTurn = true;
		}
	}

	[PunRPC]
	private async void SetStep(int stepIndex)
	{
		switch (stepIndex)
		{
			case 0: currentStep = new StartStep(this); break;
			case 1: currentStep = new JoinStep(this); break;
			case 2: currentStep = new MainStep(this); break;
			case 3: currentStep = new EndStep(this); break;
		}

		await StepSequence();
	}

	private async UniTask StepSequence()
	{
		isStepReady = false;

		await currentStep.Init();
		await currentStep.Main();

		isStepReady = true;
	}

	private void Update()
	{
		// スペースキーが押されたとき、isStepReady が true の場合にのみ処理を進行
		if (isStepReady && isPlayerTurn && Input.GetKeyDown(KeyCode.Space))
		{
			isStepReady = false;  // ここで一時的に `isStepReady` を false に設定
			photonView.RPC(nameof(ExecuteEndStep), RpcTarget.All);
		}
	}

	[PunRPC]
	private async void ExecuteEndStep()
	{
		await currentStep.End();

		// 次のステップに進む処理を `EndStep` 以外でも行う
		AdvanceStep();
	}

	private void AdvanceStep()
	{
		// 次のステップに移行
		currentStep = currentStep.GetNextStep();

		// 新しいステップが決まったら、全員に通知して進行
		photonView.RPC(nameof(SetStep), RpcTarget.All, GetStepIndex(currentStep));

		// 次のステップに進んだ後に `isStepReady` を true に設定
		isStepReady = true;

		// ターン数の加算とターン交代
		if (currentStep is EndStep)
		{
			if (isMaster)
			{
				currentTurn++; // ターン数のインクリメント
				isPlayerTurn = false; // 次のターンのプレイヤーに交代
			}
			else
			{
				isPlayerTurn = true; // 非マスターのターンに交代
			}
		}
	}

	private int GetStepIndex(Step step)
	{
		if (step is StartStep) return 0;
		if (step is JoinStep) return 1;
		if (step is MainStep) return 2;
		if (step is EndStep) return 3;
		return -1;
	}
}