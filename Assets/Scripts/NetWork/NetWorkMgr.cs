using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using static Common;
using battleTypes;
using System;
using Photon.Realtime;

public class NetWorkMgr : MonoBehaviourPunCallbacks
{
	// インスタンス
	public static NetWorkMgr instance;

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

		PhaseMgr PhaseMgr = PhaseMgr.instance;

		PhaseMgr.GetSetPhases = new Phase[phaseViewIDs.Length];

		for (int i = 0; i < phaseViewIDs.Length; i++)
		{
			PhotonView photonView = PhotonView.Find(phaseViewIDs[i]);
			if (!photonView)
			{
				Debug.LogError($"PhotonViewID {phaseViewIDs[i]} のオブジェクトが見つかりませんでした。");
				continue;
			}

			PhaseMgr.GetSetPhases[i] = photonView.GetComponent<Phase>();
			PhaseMgr.GetSetPhases[i].transform.SetParent(PhaseMgr.GetSetPhaseParentObj.transform);
		}
	}

	// フェイズ終了の通知
	[PunRPC]
	public void RPC_EndPhase_MC(int _phaseIdx, int _state)
	{
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogError($"{nameof(RPC_EndPhase_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		Debug.Log($"{nameof(RPC_EndPhase_MC)}" +
				  $"_phaseIdx：{_phaseIdx}, _state：{_state}");

		PhaseMgr PhaseMgr = PhaseMgr.instance;

		PhaseMgr.PhaseType phaseType = (PhaseMgr.PhaseType)_phaseIdx;
		Enum stateEnum = null;

		switch(phaseType)
		{
			case PhaseMgr.PhaseType.Start:
				stateEnum = (StartPhase.StartPhaseState)_state;
				break;
			case PhaseMgr.PhaseType.DiceRoll:
				stateEnum = (DiceRollPhase.DiceRollPhaseState)_state;
				break;
			case PhaseMgr.PhaseType.Main:
				stateEnum = (MainPhase.MainPhaseState)_state;
				break;
			case PhaseMgr.PhaseType.End:
				stateEnum = (EndPhase.EndPhaseState)_state;
				break;
			default:
				Debug.LogError($"{nameof(RPC_EndPhase_MC)}" +
							   $"switchで想定外の値を検知。phaseType：{phaseType}、_state：{_state}");
				break;
		}

		// フェイズ終了通知
		PhaseMgr.GetSetPhases[(int)phaseType].SwitchState(stateEnum);
	}

	//////////////////////////
	// ===== ユーザー ===== //
	//////////////////////////
	// ユーザー情報の通信同期
	[PunRPC]
    public async void RPC_SyncUser_MC(int _userSide, int _id, int _phaseType, bool _phaseReadyFlag, bool _gameStartFlag)
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
		Side		userSide	=	GetRevSide((Side)_userSide);	// ユーザー側
		PhaseType	phaseType	=	(PhaseType)_phaseType;			// 現在のフェイズ

		UserMgr UserMgr = UserMgr.Instance;
		User User = UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => User != null);

		// ユーザー設定
		if(User.GetSetID == 0)
		{
			User.GetSetID = _id;
		}
		User.GetSetPhaseType = phaseType;
		User.GetSetPhaseReadyFlag = _phaseReadyFlag;
		User.GetSetGameStartFlag = _gameStartFlag;
		Debug.Log($"{nameof(RPC_SyncUser_MC)} || User：{User.GetSetSide}, GetSetGameStartFlag：{User.GetSetGameStartFlag}");
	}

    // ユーザー情報の送信
    [PunRPC]
    public async void RPC_PushUser_CM(int _userSide, int _phaseType, bool _phaseReadyFlag, bool _gameStartFlag)
    {
		Debug.Log($"{nameof(RPC_PushUser_CM)}" +
				  $"_userSide：{_userSide}, _phaseType：{_phaseType}, _phaseReadyFlag：{_phaseReadyFlag}, _gameStartFlag：{_gameStartFlag}");
        // 非マスタークライアントならはじく
        if(!PhotonNetwork.IsMasterClient)
        {
			Debug.LogError($"{nameof(RPC_PushUser_CM)}非マスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		// 自分と相手のユーザー情報の側は逆なのでここで逆にする
		Side userSide = GetRevSide((Side)_userSide);    // ユーザー側
		PhaseType phaseType = (PhaseType)_phaseType;            // 現在のフェイズ

		UserMgr UserMgr = UserMgr.Instance;
		User User = UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => User != null);

		User.GetSetPhaseType = phaseType;
		User.GetSetPhaseReadyFlag = _phaseReadyFlag;
		User.GetSetGameStartFlag = _gameStartFlag;
		Debug.Log($"{nameof(RPC_PushUser_CM)} || User：{User.GetSetSide}, GetSetGameStartFlag：{User.GetSetGameStartFlag}");
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

		CardAbilityMgr cardAbilityMgr = CardAbilityMgr.instance;

		// カード効果種類
		if(cardAbilityMgr.GetSetAbilityTypeActDict.TryGetValue(abilityType, out Action<object[]> abilityAction))
		{
			abilityAction(_args);
		}

		await UniTask.CompletedTask;
	}
}