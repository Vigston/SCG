using UnityEngine;

public class AnimationTester : MonoBehaviour
{
	// 今後アニメーションが増えた場合はenumを拡張
	public enum AnimationType
	{
		DiceRoll,
		// ここに新しいアニメーションを追加
		// AssignCard,
	}
	public AnimationType animationType = AnimationType.DiceRoll;
}
