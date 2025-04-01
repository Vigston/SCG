using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using battleTypes;
using static Common;
using System;
using Photon.Realtime;

public class Test_NetWorkMgr : MonoBehaviourPun
{
	// インスタンス
	public static Test_NetWorkMgr instance;

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (!instance)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	//////////////////////////
	// ===== フェイズ ===== //
	//////////////////////////
	// フェイズオブジェクトの通信同期処理
	[PunRPC]
	public void RPC_SyncPhases_MC(int[] phaseViewIDs)
	{
		Debug.Log($"{nameof(RPC_SyncPhases_MC)}");
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogError($"{nameof(RPC_SyncPhases_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		PhaseManager phaseManager = PhaseManager.instance;

		phaseManager.GetSetPhases = new Phase[phaseViewIDs.Length];

		for (int i = 0; i < phaseViewIDs.Length; i++)
		{
			PhotonView photonView = PhotonView.Find(phaseViewIDs[i]);
			if (!photonView)
			{
				Debug.LogError($"PhotonViewID {phaseViewIDs[i]} のオブジェクトが見つかりませんでした。");
				continue;
			}

			phaseManager.GetSetPhases[i] = photonView.GetComponent<Phase>();
			phaseManager.GetSetPhases[i].transform.SetParent(phaseManager.GetSetPhaseParentObj.transform);
		}
	}

	//////////////////////////
	// ===== ユーザー ===== //
	//////////////////////////
	// ユーザー情報の通信同期
	[PunRPC]
    public async void RPC_SyncUser_MC(int _userSide, int _id, int _phaseType, bool _phaseReadyFlag, int _phaseState, bool _phaseStateReadyFlag)
    {
		Debug.Log($"{nameof(RPC_SyncUser_MC)}" +
				  $"_userSide：{_userSide}, _id：{_id}, _phaseType：{_phaseType}, _phaseReadyFlag：{_phaseReadyFlag}");
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError($"{nameof(RPC_SyncUser_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
            return;
        }

		// 後々選択中のカードエリアも同期してください n_oishi 2025/3/6
		// 自分と相手のユーザー情報の側は逆なのでここで逆にする
		Side					userSide	=	GetRevSide((Side)_userSide);		// ユーザー側
		PhaseManager.PhaseType	phaseType	=	(PhaseManager.PhaseType)_phaseType;	// 現在のフェイズ
		Enum					phaseState	=	null;								// 現在のステート

		// フェイズに応じて適切なEnum型に変換
		switch (phaseType)
		{
			case PhaseManager.PhaseType.Start:
				phaseState = (TestStartPhase.StartPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.Join:
				phaseState = (TestJoinPhase.JoinPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.Main:
				phaseState = (TestMainPhase.MainPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.End:
				phaseState = (TestEndPhase.EndPhaseState)_phaseState;
				break;
			default:
				Debug.LogError($"意図していないフェイズ情報を検知。確認をお願いします。：phaseType {phaseType}|| phaseState {_phaseState}");
				break;
		}

		Test_UserMgr test_UserMgr = Test_UserMgr.instance;
		Test_User test_User = test_UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => test_User != null);

		// ユーザー設定
		if(test_User.GetSetID == 0)
		{
			test_User.GetSetID = _id;
		}
		test_User.GetSetPhaseType = phaseType;
		test_User.GetSetPhaseReadyFlag = _phaseReadyFlag;
		test_User.GetSetPhaseState = phaseState;
		test_User.GetSetPhaseStateReadyFlag = _phaseStateReadyFlag;
	}

    // ユーザー情報の送信
    [PunRPC]
    public async void RPC_PushUser_CM(int _userSide, int _phaseType, bool _phaseReadyFlag, int _phaseState, bool _phaseStateReadyFlag)
    {
		Debug.Log($"{nameof(RPC_PushUser_CM)}" +
				  $"_userSide：{_userSide}, _phaseType：{_phaseType}, _phaseReadyFlag：{_phaseReadyFlag}");
        // 非マスタークライアントならはじく
        if(!PhotonNetwork.IsMasterClient)
        {
			Debug.LogError($"{nameof(RPC_PushUser_CM)}非マスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		// 自分と相手のユーザー情報の側は逆なのでここで逆にする
		Side userSide = GetRevSide((Side)_userSide);	// ユーザー側
		PhaseManager.PhaseType phaseType = (PhaseManager.PhaseType)_phaseType;	// 現在のフェイズ
		Enum phaseState = null;							// 現在のステート

		// フェイズに応じて適切なEnum型に変換
		switch (phaseType)
		{
			case PhaseManager.PhaseType.Start:
				phaseState = (TestStartPhase.StartPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.Join:
				phaseState = (TestJoinPhase.JoinPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.Main:
				phaseState = (TestMainPhase.MainPhaseState)_phaseState;
				break;
			case PhaseManager.PhaseType.End:
				phaseState = (TestEndPhase.EndPhaseState)_phaseState;
				break;
			default:
				Debug.LogError($"意図していないフェイズ情報を検知。確認をお願いします。：phaseType {phaseType}|| phaseState {_phaseState}");
				break;
		}

		Test_UserMgr test_UserMgr = Test_UserMgr.instance;
		Test_User test_User = test_UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => test_User != null);

		test_User.GetSetPhaseType = phaseType;
		test_User.GetSetPhaseReadyFlag = _phaseReadyFlag;
		test_User.GetSetPhaseState = phaseState;
		test_User.GetSetPhaseStateReadyFlag = _phaseStateReadyFlag;
	}

	/////////////////////////////
	// ===== CardAbility ===== //
	/////////////////////////////
	// 下記の型以外をobjectに入れてしまうとRPCで送信できないので辞めてください。
	// string, int, float, bool, Vector3, Quaternion, byte[], int[], string[], List<int>
	// それ以外の型を送信する場合は、自前でシリアライズして送信してください。
	// カード効果を実行
	[PunRPC]
	public async void RPC_ActivateAbility_Other(string _abilityType_str, object[] _args)
	{
		// アビリティID取得
		int abilityId = Convert.ToInt32(_args[0]);

		// 自分が発行したアビリティがRPCで送信されてくることは意図した挙動じゃないのでログを出してはじく
		if(PhotonNetwork.LocalPlayer.ActorNumber == abilityId)
		{
			Debug.LogError($"{nameof(RPC_ActivateAbility_Other)}" +
						   $"自分が発行したアビリティがRPCで送信されてきました。：{_abilityType_str}");
			return;
		}

		Type abilityType = Type.GetType(_abilityType_str);

		Debug.Log($"{nameof(RPC_ActivateAbility_Other)}" +
				  $"_abilityType：{abilityType}, _args：{_args}");

		CardAbilityManager cardAbilityMgr = CardAbilityManager.instance;

		// カード効果種類
		if(cardAbilityMgr.GetSetAbilityTypeActDict.TryGetValue(abilityType, out Action<object[]> abilityAction))
		{
			abilityAction(_args);
		}

		await UniTask.CompletedTask;
	}
}