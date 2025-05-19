using System.Collections.Generic;
using UnityEngine;

public class CardLoader
{
	private List<CardData> cardDataList;

	// JSONファイルを読み込む
	public void LoadCardData()
	{
		AssetManager.Load<TextAsset>("CardDataTable", cardDataTable =>
		{
			// JSONデータをデシリアライズ
			CardDataList dataList = JsonUtility.FromJson<CardDataList>(cardDataTable.text);
			cardDataList = dataList.cards;

			// JSONのList<Cost>をDictionaryに変換
			foreach (var card in cardDataList)
			{
				card.costs = new Dictionary<string, int>();
				foreach (var cost in card.costList)
				{
					card.costs[cost.Key] = cost.Value;
				}
			}

			// デバッグ出力
			//foreach (var card in cardDataList)
			//{
			//	string costStr = "";
			//	foreach (var c in card.costs)
			//	{
			//		costStr += $"{c.Key}:{c.Value} ";
			//	}
			//	Debug.Log($"カード名: {card.name}, コスト: {costStr}");
			//}
		},
		error =>
		{
			Debug.LogError($"カード情報の読み込み失敗: {error}");
		});
	}

	// カードデータを取得
	public List<CardData> GetCardDataList()
	{
		return cardDataList;
	}
}