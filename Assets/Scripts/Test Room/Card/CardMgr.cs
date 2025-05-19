using System.Collections.Generic;
using UnityEngine;

public class CardMgr : MonoBehaviour
{
	[SerializeField]
	private GameObject cardPrefab; // カードのプレハブ
	[SerializeField]
	private ResourceMgr resourceMgr; // 資源管理クラス
	
	private CardLoader cardLoader; // カードデータを読み込むクラス

	private void Awake()
	{
		// カードローダーのインスタンス生成
		if(cardLoader == null)
		{
			cardLoader = new CardLoader();
		}

		// カードデータを読み込む
		cardLoader.LoadCardData();
	}

	// カードを生成
	public GameObject CreateCard(string cardName)
	{
		if (cardPrefab == null)
		{
			Debug.LogError("Card プレハブが設定されていません！");
			return null;
		}

		// カードデータを取得
		CardData cardData = GetCardDataByName(cardName);
		if (cardData == null)
		{
			Debug.LogError($"カード {cardName} のデータが見つかりません！");
			return null;
		}

		// コストを確認
		if (!CanPayCost(cardData.costs))
		{
			Debug.LogWarning($"カード {cardData.name} を生成するための資源が不足しています！");
			return null;
		}

		// コストを消費
		PayCost(cardData.costs);

		// カードを生成
		GameObject cardObject = Instantiate(cardPrefab);
		Card cardComponent = cardObject.GetComponent<Card>();

		if (cardComponent != null)
		{
			// カードのプロパティを設定
			cardComponent.Initialize(cardData.name, cardData.costs);
		}
		else
		{
			Debug.LogError("生成されたオブジェクトに Card コンポーネントがアタッチされていません！");
		}

		Debug.Log($"カード {cardData.name} を生成しました！");
		return cardObject;
	}

	// カードデータを名前で検索
	private CardData GetCardDataByName(string cardName)
	{
		List<CardData> cardDataList = cardLoader.GetCardDataList();
		return cardDataList.Find(card => card.name == cardName);
	}

	// コストを支払えるか確認
	private bool CanPayCost(Dictionary<string, int> cost)
	{
		foreach (var resource in cost)
		{
			if (resourceMgr.GetResourceQuantity(resource.Key) < resource.Value)
			{
				return false; // 資源が不足している
			}
		}
		return true;
	}

	// コストを消費
	private void PayCost(Dictionary<string, int> cost)
	{
		foreach (var resource in cost)
		{
			resourceMgr.ConsumeResource(resource.Key, resource.Value);
		}
	}
}
