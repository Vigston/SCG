using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class PhaseManager : MonoBehaviourPunCallbacks
{
	private Phase[] phases;
	public int currentPhaseIndex = 0;

	private void Start()
	{
		phases = new Phase[]
		{
			new TestStartPhase(),
			new TestJoinPhase(),
			new TestMainPhase(),
			new TestEndPhase()
		};

		RunTurnCycle().Forget();
	}

	private async UniTask RunTurnCycle()
	{
		while (true)
		{
			await phases[currentPhaseIndex].RunPhase();

			currentPhaseIndex++;
			if (currentPhaseIndex >= phases.Length)
			{
				currentPhaseIndex = 0; // 次のターンへ
				Debug.Log("Next Turn");
			}

			await UniTask.Yield();
		}
	}
}